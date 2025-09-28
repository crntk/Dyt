using Dyt.Business.Background;
using Dyt.Business.Security.Sanitization;
using Dyt.Business.Security.Url;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHostedService<ReminderHostedService>(); // Arka plan hatýrlatma servisini ekliyorum
builder.Services.AddScoped<IContentSanitizer, ContentSanitizer>(); // Ýçerik temizleyiciyi DI'a ekliyorum
builder.Services.AddSingleton<ProfanityFilter>(); // Küfür filtresini tekil örnek olarak ekliyorum
builder.Services.AddScoped<ISignedUrlService, SignedUrlService>(); // Ýmzalý URL servis kaydý yapýyorum

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();     

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
