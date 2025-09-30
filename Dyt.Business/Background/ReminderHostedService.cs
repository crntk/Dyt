using Dyt.Business.Interfaces.Appointments; // Onay token servisi için ekliyorum
using Dyt.Business.Interfaces.Notifications; // SMS ve şablon servisi için ekliyorum
using Dyt.Business.Options; // Hatırlatma ayarlarını okumak için ekliyorum
using Dyt.Business.Security.Url; // İmzalı URL üretimi için ekliyorum
using Dyt.Business.Utils; // Zaman sağlayıcısı için ekliyorum
using Dyt.Data.Context; // DbContext tipini görmek için ekliyorum
using Dyt.Data.Enums; // Enum'lar için ekliyorum
using Microsoft.EntityFrameworkCore; // EF sorguları için ekliyorum
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting; // BackgroundService tabanı için ekliyorum
using Microsoft.Extensions.Logging; // Loglama için ekliyorum
using Microsoft.Extensions.Options; // IOptions için ekliyorum

namespace Dyt.Business.Background // Arka plan işleri için ad alanını koruyorum
{
    /// <summary>
    /// Yaklaşan randevular için SMS hatırlatma ve 2 saat kala yanıt yok uyarısını işleyen servis.
    /// DbContext scoped olduğu için doğrudan enjekte edilmez; her çalışmada DI scope oluşturulur.
    /// </summary>
    public class ReminderHostedService : BackgroundService // Arka plan servis tabanı
    {
        private readonly ILogger<ReminderHostedService> _log; // Logger alanı
        private readonly IServiceScopeFactory _scopeFactory; // Yeni scope yaratmak için fabrika
        private readonly IDateTimeProvider _clock; // Zaman sağlayıcısı
        private readonly ISmsSender _sms; // SMS gönderici
        private readonly INotificationTemplateService _tpl; // Şablon servisi
        private readonly IConfirmationTokenService _tokens; // Onay token servisi
        private readonly ISignedUrlService _urls; // İmzalı URL servisi
        private readonly ReminderOptions _opt; // Hatırlatma ayarları

        /// <summary>
        /// Bağımlılıkları alarak servisi başlatır. DbContext yerine IServiceScopeFactory enjekte edilir.
        /// </summary>
        public ReminderHostedService(
            ILogger<ReminderHostedService> log,             // Loglama için alıyorum
            IServiceScopeFactory scopeFactory,              // Scoped servisleri elde etmek için alıyorum
            IDateTimeProvider clock,                        // Zaman sağlayıcısını alıyorum
            ISmsSender sms,                                 // SMS göndericiyi alıyorum
            INotificationTemplateService tpl,               // Şablon servisini alıyorum
            IConfirmationTokenService tokens,               // Token servisini alıyorum
            ISignedUrlService urls,                         // URL üreticiyi alıyorum
            IOptions<ReminderOptions> opt)                  // Ayarları IOptions ile alıyorum
        {
            _log = log;                 // Logger'ı saklıyorum
            _scopeFactory = scopeFactory; // Scope fabrikasını saklıyorum
            _clock = clock;             // Zaman sağlayıcısını saklıyorum
            _sms = sms;                 // SMS göndericiyi saklıyorum
            _tpl = tpl;                 // Şablon servisini saklıyorum
            _tokens = tokens;           // Token servisini saklıyorum
            _urls = urls;               // URL servisini saklıyorum
            _opt = opt.Value;           // Ayar değerini saklıyorum
        }

        /// <summary>
        /// Sonsuz döngü içinde belirlenen aralıklarla hatırlatma/uyarı işlemlerini yürütür.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) // Çalışma döngüsünü başlatıyorum
        {
            var delay = TimeSpan.FromSeconds(Math.Max(30, _opt.ScanIntervalSeconds)); // Minimum 30 sn gecikme
            while (!stoppingToken.IsCancellationRequested) // İptal edilmedikçe döngü
            {
                try // Hataları izole ediyorum
                {
                    await ProcessRemindersAsync(stoppingToken); // Asıl işi yapıyorum
                }
                catch (Exception ex) // Hata olursa
                {
                    _log.LogError(ex, "Hatırlatma servisi çalışırken hata oluştu"); // Hata logluyorum
                }

                await Task.Delay(delay, stoppingToken); // Sonraki turu bekliyorum
            }
        }

        /// <summary>
        /// Tek seferlik hatırlatma ve 2 saat kala uyarı işlerini yapar.
        /// Her çalışmada yeni bir scope açar ve DbContext’i scope içinden alır.
        /// </summary>
        private async Task ProcessRemindersAsync(CancellationToken ct) // İş mantığını kapsülleyen metot
        {
            using var scope = _scopeFactory.CreateScope(); // Yeni bir DI scope oluşturuyorum
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Scoped DbContext'i buradan alıyorum

            var now = _clock.UtcNow; // Şu an UTC
            var remindAtFrom = now.AddHours(_opt.HoursBefore - 0.25); // 15 dk tolerans
            var remindAtTo = now.AddHours(_opt.HoursBefore + 0.25); // 15 dk tolerans

            // Hatırlatma penceresindeki randevuları çekiyorum
            var toRemind = await db.Appointments
                .AsNoTracking()
                .Where(a => a.Status == AppointmentStatus.Scheduled)
                .Where(a => a.ConfirmationState == ConfirmationState.Yanıtlanmadı)
                .Where(a =>
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc) >= remindAtFrom &&
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc) <= remindAtTo)
                .Select(a => new { a.Id, a.ClientName, a.ClientPhone, a.AppointmentDate, a.StartTime })
                .ToListAsync(ct);

            foreach (var a in toRemind) // Her randevu için
            {
                var exp = DateTimeOffset.UtcNow.AddHours(_opt.HoursBefore); // Token ömrü
                var tYes = _tokens.GenerateYesToken(a.Id, exp); // Evet token
                var tNo = _tokens.GenerateNoToken(a.Id, exp);  // Hayır token

                var yesUrl = _urls.Build("/appointment/confirm", new Dictionary<string, string?> { ["token"] = tYes, ["intent"] = "yes" }); // Evet URL
                var noUrl = _urls.Build("/appointment/confirm", new Dictionary<string, string?> { ["token"] = tNo, ["intent"] = "no" }); // Hayır URL

                var message = _tpl.RenderReminder(a.ClientName, a.AppointmentDate, a.StartTime, yesUrl, noUrl); // SMS metni
                _ = await _sms.SendAsync(a.ClientPhone, message, ct); // Mock gönderim
                _log.LogInformation("Hatırlatma SMS gönderildi. AppointmentId={Id}", a.Id); // Log
            }

            // 2 saat kala hâlâ yanıt yoksa uyarı kaydı (şimdilik log)
            var twoFrom = now.AddHours(2 - 0.25); // 15 dk tolerans
            var twoTo = now.AddHours(2 + 0.25); // 15 dk tolerans

            var twoHourAlerts = await db.Appointments
                .AsNoTracking()
                .Where(a => a.Status == AppointmentStatus.Scheduled)
                .Where(a => a.ConfirmationState == ConfirmationState.Yanıtlanmadı)
                .Where(a =>
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc) >= twoFrom &&
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc) <= twoTo)
                .Select(a => new { a.Id, a.ClientName, a.AppointmentDate, a.StartTime })
                .ToListAsync(ct);

            foreach (var a in twoHourAlerts) // Uyarı log
            {
                var msg = _tpl.RenderTwoHourNoResponseAlert(a.ClientName, a.AppointmentDate, a.StartTime); // Uyarı metni
                _log.LogWarning("2 saat kala yanıt yok: {Msg} (AppointmentId={Id})", msg, a.Id); // Log
            }
        }
    }
}
