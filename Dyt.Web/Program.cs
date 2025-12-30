// -----------------------------------------------------------------------------
// Program.cs
// Uygulamanın giriş noktası: servis kayıtları (DI), Identity + Cookie ayarları,
// DbContext/Options bağlama, MVC yönlendirme ve arka plan servisleri burada yapılır.
// -----------------------------------------------------------------------------

// Business katmanı: arka plan iş, arayüzler, uygulamalar ve yardımcılar için gerekli using'leri ekliyorum
using Dyt.Business.Background;                 // ReminderHostedService için
using Dyt.Business.Interfaces.Appointments;    // IAppointmentService için
using Dyt.Business.Interfaces.Notifications;   // ISmsSender, INotificationTemplateService için
using Dyt.Business.Interfaces.Scheduling;      // IScheduleService için
using Dyt.Business.Options;                    // ReminderOptions, SmsOptions, SecurityOptions için
using Dyt.Business.Security.Sanitization;      // IContentSanitizer için
using Dyt.Business.Security.Url;               // ISignedUrlService için
using Dyt.Business.Services.Appointments;      // AppointmentService, ConfirmationTokenService için
using Dyt.Business.Services.Notifications;     // SmsSenderMock, NotificationTemplateService için
using Dyt.Business.Services.Scheduling;        // ScheduleService için
using Dyt.Business.Utils;                      // IDateTimeProvider, DateTimeProvider için

// Data katmanı: DbContext ve Identity varlıkları için gerekli using'leri ekliyorum
using Dyt.Data.Context;                        // AppDbContext için
using Dyt.Data.Entities.Identity;              // AppUser, AppRole için

// ASP.NET Core: Identity, EF Core ve MVC altyapısı için gerekli using'leri ekliyorum
using Microsoft.AspNetCore.Identity;           // Identity servisleri için
using Microsoft.EntityFrameworkCore;           // UseSqlServer için

// Web katmanı: başlangıç admin kullanıcısını seed eden yardımcı sınıfı almak için ekliyorum
using Dyt.Web.Infrastructure;                  // AdminSeeder için

// Builder nesnesini oluşturuyorum; konfigürasyon ve servis kayıtlarını bunun üzerinden yapacağım
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// DbContext kaydı: SQL Server bağlantısını DI konteynerine ekliyorum
// -----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("Default"); // appsettings.json: ConnectionStrings:Default değerini okuyorum
builder.Services.AddDbContext<AppDbContext>(opt =>                           // DbContext'i DI'a ekliyorum
{
    opt.UseSqlServer(connectionString);                                      // SQL Server sağlayıcısını seçiyorum
    // İstersen audit/soft-delete gibi interceptor'ları burada ekleyebilirsin:
    // opt.AddInterceptors(new SaveChangesAuditingInterceptor());
});

// -----------------------------------------------------------------------------
// Identity + Cookie kimlik doğrulama ayarları
// -----------------------------------------------------------------------------
builder.Services
    .AddIdentity<AppUser, AppRole>(opt =>               // Identity altyapısını uygulamaya ekliyorum
    {
        opt.Password.RequiredLength = 8;                // Parola minimum uzunluğu
        opt.Password.RequireDigit = false;              // Rakam zorunlu olmasın
        opt.Password.RequireNonAlphanumeric = false;    // Özel karakter zorunlu olmasın
        opt.Password.RequireUppercase = false;          // Büyük harf zorunlu olmasın
        opt.Password.RequireLowercase = false;          // Küçük harf zorunlu olmasın
        opt.SignIn.RequireConfirmedEmail = false;       // E-posta onayı gerektirmiyorum (tek kullanıcı senaryosu)
    })
    .AddEntityFrameworkStores<AppDbContext>()           // Kimlik verilerini EF Core üzerinden saklayacağımı belirtiyorum
    .AddDefaultTokenProviders();                        // Parola sıfırlama vb. token sağlayıcılarını ekliyorum

builder.Services.ConfigureApplicationCookie(c =>        // Uygulama çerez ayarlarını yapıyorum
{
    c.LoginPath = "/Account/Login";                     // Yetkisiz isteklerde yönlenecek adres
    c.AccessDeniedPath = "/Account/Denied";             // Yetki yoksa gösterilecek adres
    c.Cookie.Name = "DytAuth";                          // Kimlik çerezinin adı
    c.Cookie.HttpOnly = true;                           // JS tarafından okunamaz
    c.SlidingExpiration = true;                         // Kullanıldıkça süre uzasın
    c.ExpireTimeSpan = TimeSpan.FromDays(14);           // Oturum süresi
});

// -----------------------------------------------------------------------------
// MVC (Controllers + Views)
// -----------------------------------------------------------------------------
builder.Services.AddControllersWithViews();             // Controller ve View desteğini ekliyorum

// -----------------------------------------------------------------------------
// Business servis kayıtları (DI)
// Not: HostedService singleton çalışır; bu yüzden stateless servisleri singleton,
// DbContext kullanan servisleri scoped olarak kaydediyorum.
// -----------------------------------------------------------------------------
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();              // Zaman sağlayıcısı (stateless)
builder.Services.AddScoped<IScheduleService, ScheduleService>();                   // Çalışma saatleri/slot hesaplama (DbContext kullanır)
builder.Services.AddScoped<IAppointmentService, AppointmentService>();             // Randevu iş akışları (DbContext kullanır)
builder.Services.AddSingleton<IConfirmationTokenService, ConfirmationTokenService>(); // Onay token servisi (stateless)
builder.Services.AddSingleton<ISmsSender, SmsSenderMock>();                        // SMS gönderici (şimdilik mock, stateless)
builder.Services.AddSingleton<INotificationTemplateService, NotificationTemplateService>(); // SMS şablon üretimi (stateless)
builder.Services.AddScoped<IContentSanitizer, ContentSanitizer>();                 // İçerik temizleyici (XSS'e karşı)
builder.Services.AddSingleton<ISignedUrlService, SignedUrlService>();              // İmzalı URL üretimi (stateless)
builder.Services.AddSingleton<IEmailSender, EmailSenderStub>();                    // E-posta gönderici stub
builder.Services.AddHostedService<ReminderHostedService>();                        // Arka plan hatırlatma servisini ekliyorum

// -----------------------------------------------------------------------------
// Options (appsettings.json bölümlerini bağlıyorum)
// -----------------------------------------------------------------------------
builder.Services.Configure<ReminderOptions>(builder.Configuration.GetSection(ReminderOptions.SectionName)); // "Reminder" bölümü
builder.Services.Configure<SmsOptions>(builder.Configuration.GetSection(SmsOptions.SectionName));           // "Sms" bölümü
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.SectionName)); // "Security" bölümü

// Uygulama nesnesini oluşturuyorum; bundan sonra middleware zincirini yapılandıracağım
var app = builder.Build(); // <-- WebApplication.Build(builder) değil

// -----------------------------------------------------------------------------
// HTTP request pipeline (middleware zinciri)
// -----------------------------------------------------------------------------
if (!app.Environment.IsDevelopment())                        // Üretim benzeri ortamda
{
    app.UseExceptionHandler("/Home/Error");                  // Global hata sayfası
    app.UseHsts();                                           // HTTP Strict Transport Security
}

app.UseHttpsRedirection();                                   // HTTP → HTTPS yönlendirmesi
app.UseStaticFiles();                                        // wwwroot altındaki statik dosyalara izin veriyorum
app.UseRouting();                                            // Rota altyapısını devreye alıyorum
app.UseAuthentication();                                     // Kimlik doğrulamayı zincire ekliyorum
app.UseAuthorization();                                      // Yetkilendirmeyi zincire ekliyorum

// Root path artık Home/Index
app.MapControllerRoute(
    name: "root",
    pattern: string.Empty,
    defaults: new { controller = "Home", action = "Index" });

// Özel Admin giriş route'u
app.MapControllerRoute(
    name: "adminLogin",
    pattern: "diyetisyen",
    defaults: new { controller = "Account", action = "Login" });

// Area yönlendirmesi (Admin gibi alanlar için önce area route'u ekliyorum)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// Varsayılan MVC rotası: Misafir anasayfası Home/Index olsun
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -----------------------------------------------------------------------------
// Seed: Başlangıç admin kullanıcısını oluşturuyorum (tek seferlik)
// -----------------------------------------------------------------------------
await AdminSeeder.SeedAsync(app.Services);                    // DI üzerinden seed çalıştırıyorum

// Uygulamayı çalıştırıyorum
app.Run();
