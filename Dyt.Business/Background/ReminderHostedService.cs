using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Business.Interfaces.Appointments; // Onay token servisini kullanmak için ekliyorum
using Dyt.Business.Interfaces.Notifications; // SMS gönderici ve şablon servisini kullanmak için ekliyorum
using Dyt.Business.Options; // Hatırlatma zamanlaması için ayarlara erişmek için ekliyorum
using Dyt.Business.Security.Url; // İmzalı URL üretimi için ekliyorum
using Dyt.Business.Utils; // Zaman sağlayıcısı için ekliyorum
using Dyt.Data.Context; // Veritabanı bağlamına erişmek için ekliyorum
using Dyt.Data.Enums; // Randevu durum ve onay enum'ları için ekliyorum
using Microsoft.EntityFrameworkCore; // EF Core sorguları için ekliyorum
using Microsoft.Extensions.Hosting; // IHostedService için ekliyorum
using Microsoft.Extensions.Logging; // Loglama için ekliyorum
using Microsoft.Extensions.Options; // IOptions desenini kullanmak için ekliyorum

namespace Dyt.Business.Background // Arka plan işleri için ad alanını belirliyorum
{
    /// <summary>
    /// Belirli aralıklarla çalışarak yaklaşan randevular için SMS hatırlatması gönderir
    /// ve randevuya 2 saat kala hâlâ yanıt yoksa kontrol paneli uyarısı için log kaydı bırakır.
    /// SMS metni şablondan üretilir; onay/ret linkleri tek kullanımlık ve süreli token içerir.
    /// </summary>
    public class ReminderHostedService : BackgroundService // IHostedService'in hazır taban sınıfını kullanıyorum
    {
        private readonly ILogger<ReminderHostedService> _log; // Günlükleme için logger tutuyorum
        private readonly AppDbContext _db; // Veritabanı bağlamını tutuyorum
        private readonly IDateTimeProvider _clock; // Zaman sağlayıcısı tutuyorum
        private readonly ISmsSender _sms; // SMS göndericiyi tutuyorum
        private readonly INotificationTemplateService _tpl; // Şablonlayıcı servisi tutuyorum
        private readonly IConfirmationTokenService _tokens; // Onay token servisini tutuyorum
        private readonly ISignedUrlService _urls; // İmzalı URL üretim servisini tutuyorum
        private readonly ReminderOptions _opt; // Hatırlatma ayarlarını tutuyorum

        /// <summary>
        /// Gerekli tüm bağımlılıkları alarak servisi başlatır.
        /// </summary>
        public ReminderHostedService( // Kurucu metodu tanımlıyorum
            ILogger<ReminderHostedService> log, // Logger'ı DI'dan alıyorum
            AppDbContext db, // DbContext'i alıyorum
            IDateTimeProvider clock, // Zaman sağlayıcısını alıyorum
            ISmsSender sms, // SMS göndericiyi alıyorum
            INotificationTemplateService tpl, // Şablon üreticiyi alıyorum
            IConfirmationTokenService tokens, // Token üreticiyi alıyorum
            ISignedUrlService urls, // İmzalı URL üreticiyi alıyorum
            IOptions<ReminderOptions> opt) // Hatırlatma ayarlarını IOptions ile alıyorum
        {
            _log = log; // Logger'ı alan değişkenine atıyorum
            _db = db; // DbContext'i alan değişkenine atıyorum
            _clock = clock; // Zaman sağlayıcısını alan değişkenine atıyorum
            _sms = sms; // SMS servisini alan değişkenine atıyorum
            _tpl = tpl; // Şablon servisini alan değişkenine atıyorum
            _tokens = tokens; // Token servisini alan değişkene atıyorum
            _urls = urls; // URL servisini alan değişkene atıyorum
            _opt = opt.Value; // Ayar değerini alan değişkenine atıyorum
        }

        /// <summary>
        /// Arka plan döngüsünü başlatır; belirtilen aralıklarla kontrol yapar.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken) // Çalışma döngüsünü override ediyorum
        {
            var delay = TimeSpan.FromSeconds(Math.Max(30, _opt.ScanIntervalSeconds)); // Tarama aralığını saniyeden TimeSpan'e çeviriyorum (minimum 30 sn)
            while (!stoppingToken.IsCancellationRequested) // Servis sonlandırılmadığı sürece döngüyü sürdürüyorum
            {
                try // Hataları izole etmek için try bloğu açıyorum
                {
                    await ProcessRemindersAsync(stoppingToken); // Hatırlatma ve uyarıları işliyorum
                }
                catch (Exception ex) // Hata oluşursa
                {
                    _log.LogError(ex, "Hatırlatma servisi çalışırken hata oluştu"); // Hata logluyorum
                }
                await Task.Delay(delay, stoppingToken); // Belirlenen aralık kadar bekliyorum
            }
        }

        /// <summary>
        /// Yaklaşan randevular için SMS hatırlatmaları gönderir ve 2 saat kala yanıt yok uyarısı üretir.
        /// </summary>
        private async Task ProcessRemindersAsync(CancellationToken ct) // Esas iş mantığını ayrı metoda alıyorum
        {
            var now = _clock.UtcNow; // Şu anki UTC zamanını alıyorum
            var remindAtFrom = now.AddHours(_opt.HoursBefore - 0.25); // 15 dk toleransla alt sınırı hesaplıyorum
            var remindAtTo = now.AddHours(_opt.HoursBefore + 0.25); // 15 dk toleransla üst sınırı hesaplıyorum

            // Hatırlatma penceresine giren randevuları çekiyorum
            var toRemind = await _db.Appointments
                .AsNoTracking() // Okuma amaçlı olduğu için takip açmıyorum
                .Where(a => a.Status == AppointmentStatus.Scheduled) // Sadece planlı randevular
                .Where(a => a.ConfirmationState == ConfirmationState.Yanıtlanmadı) // Hâlâ yanıt yoksa
                .Where(a =>
                    // Tarih-saat birleştirip aralığa düşeni buluyorum
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc)
                    >= remindAtFrom &&
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc)
                    <= remindAtTo)
                .Select(a => new { a.Id, a.ClientName, a.ClientPhone, a.AppointmentDate, a.StartTime }) // Gerekli alanları seçiyorum
                .ToListAsync(ct); // Listeye alıyorum

            foreach (var a in toRemind) // Her randevu için döngüye giriyorum
            {
                var ttl = TimeSpan.FromMinutes(_opt.HoursBefore * 60); // Token ömrünü saatten dakikaya çeviriyorum
                var exp = DateTimeOffset.UtcNow.Add(ttl); // Token son kullanım zamanını hesaplıyorum

                var tYes = _tokens.GenerateYesToken(a.Id, exp); // Evet token'ını üretiyorum
                var tNo = _tokens.GenerateNoToken(a.Id, exp); // Hayır token'ını üretiyorum

                var yesUrl = _urls.Build("/appointment/confirm", new Dictionary<string, string?> // Evet linkini üretiyorum
                {
                    ["token"] = tYes, // Token parametresini ekliyorum
                    ["intent"] = "yes" // Niyet bilgisini ekliyorum
                });

                var noUrl = _urls.Build("/appointment/confirm", new Dictionary<string, string?> // Hayır linkini üretiyorum
                {
                    ["token"] = tNo, // Token parametresini ekliyorum
                    ["intent"] = "no" // Niyet bilgisini ekliyorum
                });

                var message = _tpl.RenderReminder(a.ClientName, a.AppointmentDate, a.StartTime, yesUrl, noUrl); // Şablondan SMS metnini üretiyorum
                var _ = await _sms.SendAsync(a.ClientPhone, message, ct); // SMS gönderimini çağırıyorum (dönüş bilgisi loglanabilir)
                _log.LogInformation("Hatırlatma SMS gönderildi. AppointmentId={Id}", a.Id); // Bilgi logu yazıyorum
            }

            // 2 saat kala hâlâ yanıt yoksa kontrol paneline uyarı düşmek için log kaydı bırakıyorum (örn. ileride SignalR ile gösterilebilir)
            var twoHoursFrom = now.AddHours(2 - 0.25); // 15 dk tolerans ile alt sınırı hesaplıyorum
            var twoHoursTo = now.AddHours(2 + 0.25); // Üst sınırı hesaplıyorum

            var twoHourAlerts = await _db.Appointments
                .AsNoTracking() // Takip yok
                .Where(a => a.Status == AppointmentStatus.Scheduled) // Planlı randevular
                .Where(a => a.ConfirmationState == ConfirmationState.Yanıtlanmadı) // Hâlâ yanıt yok
                .Where(a =>
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc)
                    >= twoHoursFrom &&
                    DateTime.SpecifyKind(new DateTime(a.AppointmentDate.Year, a.AppointmentDate.Month, a.AppointmentDate.Day, a.StartTime.Hour, a.StartTime.Minute, 0), DateTimeKind.Utc)
                    <= twoHoursTo)
                .Select(a => new { a.Id, a.ClientName, a.AppointmentDate, a.StartTime })
                .ToListAsync(ct); // Listeye alıyorum

            foreach (var a in twoHourAlerts) // Her uyarı için dönüyorum
            {
                var msg = _tpl.RenderTwoHourNoResponseAlert(a.ClientName, a.AppointmentDate, a.StartTime); // Uyarı metnini oluşturuyorum
                _log.LogWarning("2 saat kala yanıt yok: {Msg} (AppointmentId={Id})", msg, a.Id); // Şimdilik log olarak yazıyorum
                // Not: İleride bu noktada kontrol paneli için Notification tablosuna kayıt atılabilir.
            }
        }
    }
}

