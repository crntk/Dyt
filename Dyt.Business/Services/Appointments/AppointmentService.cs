using Dyt.Business.Interfaces.Appointments; // Arayüzleri kullanmak için ekliyorum
using Dyt.Business.Interfaces.Scheduling;   // Uygunluk kontrolü için ekliyorum
using Dyt.Business.Utils;                   // Zaman sağlayıcısı için ekliyorum
using Dyt.Data.Context;                     // DbContext'e erişmek için ekliyorum
using Dyt.Data.Entities.Scheduling;         // Appointment varlığı için ekliyorum
using Dyt.Data.Enums;                       // Durum ve onay enumları için ekliyorum
using Microsoft.EntityFrameworkCore;        // EF Core işlemleri için ekliyorum
using Microsoft.Extensions.Logging;         // Günlükleme için ekliyorum

namespace Dyt.Business.Services.Appointments // Servis implementasyonlarının ad alanını tanımlıyorum
{
    /// <summary>
    /// Randevu oluşturma, okuma ve durum güncelleme işlevlerini sağlayan servis.
    /// </summary>
    public class AppointmentService : IAppointmentService // Arayüzü uygulayan sınıfı bildiriyorum
    {
        private readonly AppDbContext _db;                   // Veritabanı bağlamını saklamak için alan tanımlıyorum
        private readonly IScheduleService _schedule;         // Uygunluk servisini saklamak için alan tanımlıyorum
        private readonly IConfirmationTokenService _tokens;  // Onay token servisini saklamak için alan tanımlıyorum
        private readonly IDateTimeProvider _clock;           // Zaman sağlayıcısını saklamak için alan tanımlıyorum
        private readonly ILogger<AppointmentService> _log;   // Günlükleme için logger alanı tanımlıyorum

        /// <summary>
        /// Gerekli bağımlılıkları alarak servisi başlatır.
        /// </summary>
        public AppointmentService(AppDbContext db, IScheduleService schedule, IConfirmationTokenService tokens, IDateTimeProvider clock, ILogger<AppointmentService> log) // Kurucu metodu tanımlıyorum
        {
            _db = db;           // DbContext bağımlılığını alana atıyorum
            _schedule = schedule; // Uygunluk servisini alana atıyorum
            _tokens = tokens;   // Token servisini alana atıyorum
            _clock = clock;     // Zaman sağlayıcısını alana atıyorum
            _log = log;         // Logger'ı alana atıyorum
        }

        /// <summary>
        /// Yeni randevu oluşturur. Çakışma kontrolü yapar.
        /// </summary>
        public async Task<Dyt.Contracts.Appointments.Responses.AppointmentDto>
            CreateAsync(Dyt.Contracts.Appointments.Requests.AppointmentCreateRequest request, CancellationToken ct = default) // Oluşturma metodunu uyguluyorum
        {
            var slotMinutes = _schedule.GetDefaultSlotMinutes(); // Slot süresini alıyorum
            var end = request.StartTime.AddMinutes(slotMinutes); // Bitiş saatini hesaplıyorum

            var ok = await _schedule.IsSlotAvailableAsync(request.Date, request.StartTime, end, ct); // Uygunluk kontrolü yapıyorum
            if (!ok) // Müsait değilse
                throw new InvalidOperationException("Seçilen saat aralığı uygun değil."); // Hata fırlatıyorum

            var exists = await _db.Appointments // Aynı saatte kayıt var mı bakıyorum
                .AnyAsync(a => a.AppointmentDate == request.Date && a.StartTime == request.StartTime, ct); // Tarih + başlama saatine göre arıyorum
            if (exists) // Varsa
                throw new InvalidOperationException("Bu saat için zaten randevu mevcut."); // Hata fırlatıyorum

            var entity = new Appointment // Yeni entity oluşturuyorum
            {
                AppointmentDate = request.Date,        // Gün bilgisini set ediyorum
                StartTime = request.StartTime,         // Başlangıç saatini set ediyorum
                EndTime = end,                         // Bitiş saatini set ediyorum
                ClientName = request.ClientName.Trim(), // İsim bilgisini kırpıp set ediyorum
                ClientPhone = request.ClientPhone.Trim(), // Telefon bilgisini kırpıp set ediyorum
                ClientEmail = request.ClientEmail?.Trim(), // E-posta varsa kırpıp set ediyorum
                Status = AppointmentStatus.Scheduled,         // Başlangıç durumunu planlandı yapıyorum
                ConfirmationState = ConfirmationState.Yanıtlanmadı // Onay durumunu yanıtlanmadı yapıyorum
            };

            await _db.Appointments.AddAsync(entity, ct); // Kaydı bağlama ekliyorum
            await _db.SaveChangesAsync(ct);              // Veritabanına yazıyorum

            _log.LogInformation("Randevu oluşturuldu: {Id} {Date} {Start}", entity.Id, entity.AppointmentDate, entity.StartTime); // Bilgi logu yazıyorum

            return new Dyt.Contracts.Appointments.Responses.AppointmentDto // DTO hazırlayıp döndürüyorum
            {
                Id = entity.Id,                         // Kimliği set ediyorum
                Date = entity.AppointmentDate,          // Tarihi set ediyorum
                StartTime = entity.StartTime,           // Başlangıcı set ediyorum
                EndTime = entity.EndTime,               // Bitişi set ediyorum
                ClientName = entity.ClientName,         // İsim set ediyorum
                ClientPhone = entity.ClientPhone,       // Telefon set ediyorum
                ClientEmail = entity.ClientEmail,       // E-posta set ediyorum
                Status = entity.Status.ToString(),      // Durumu metin olarak set ediyorum
                ConfirmationState = entity.ConfirmationState.ToString() // Onay durumunu metin olarak set ediyorum
            };
        }

        /// <summary>
        /// Kimliğe göre randevu döner; bulunamazsa null döner.
        /// </summary>
        public async Task<Dyt.Contracts.Appointments.Responses.AppointmentDto?>
            GetByIdAsync(int id, CancellationToken ct = default) // Get metodunu uyguluyorum
        {
            var a = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct); // Randevuyu çekiyorum
            if (a == null) return null; // Bulunamazsa null döndürüyorum

            return new Dyt.Contracts.Appointments.Responses.AppointmentDto // DTO'ya projekte edip döndürüyorum
            {
                Id = a.Id,                          // Kimliği set ediyorum
                Date = a.AppointmentDate,           // Tarihi set ediyorum
                StartTime = a.StartTime,            // Başlangıcı set ediyorum
                EndTime = a.EndTime,                // Bitişi set ediyorum
                ClientName = a.ClientName,          // İsim set ediyorum
                ClientPhone = a.ClientPhone,        // Telefon set ediyorum
                ClientEmail = a.ClientEmail,        // E-posta set ediyorum
                Status = a.Status.ToString(),       // Durumu set ediyorum
                ConfirmationState = a.ConfirmationState.ToString() // Onay durumunu set ediyorum
            };
        }

        /// <summary>
        /// Randevuyu "gelmedi" olarak işaretler.
        /// </summary>
        public async Task<bool> MarkStatusNoShowAsync(int id, CancellationToken ct = default) // NoShow metodunu uyguluyorum
        {
            var a = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct); // Kaydı buluyorum
            if (a == null) return false; // Yoksa false döndürüyorum

            a.Status = AppointmentStatus.NoShow; // Durumu NoShow yapıyorum
            _db.Appointments.Update(a);          // Güncelleme işaretliyorum
            await _db.SaveChangesAsync(ct);      // Kaydediyorum
            return true;                         // Başarılı sonucu döndürüyorum
        }

        /// <summary>
        /// Randevuyu iptal eder.
        /// </summary>
        public async Task<bool> CancelAsync(int id, CancellationToken ct = default) // İptal metodunu uyguluyorum
        {
            var a = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct); // Kaydı buluyorum
            if (a == null) return false; // Yoksa false döndürüyorum

            a.Status = AppointmentStatus.Canceled; // Durumu iptal yapıyorum
            _db.Appointments.Update(a);            // Güncelleme işaretliyorum
            await _db.SaveChangesAsync(ct);        // Kaydediyorum
            return true;                            // Başarılı sonucu döndürüyorum
        }

        /// <summary>
        /// Onay linkindeki token ve intent'e göre randevunun onay durumunu günceller.
        /// intent "yes" ise Gelecek, "no" ise Gelmeyecek olarak işaretler.
        /// </summary>
        public async Task<bool> ProcessConfirmationAsync(string token, string intent, CancellationToken ct = default) // Onay işleme metodunu uyguluyorum
        {
            var (ok, appointmentId) = _tokens.Validate(token); // Token'ı doğrulayıp randevu kimliğini alıyorum
            if (!ok) return false; // Geçersizse false döndürüyorum

            var a = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == appointmentId, ct); // Randevuyu buluyorum
            if (a == null) return false; // Bulunamazsa false döndürüyorum
            if (a.Status != AppointmentStatus.Scheduled) return false; // Planlı değilse değiştirmiyorum

            if (string.Equals(intent, "yes", StringComparison.OrdinalIgnoreCase)) // Intent evet ise
                a.ConfirmationState = ConfirmationState.Gelecek; // Gelecek olarak işaretliyorum
            else if (string.Equals(intent, "no", StringComparison.OrdinalIgnoreCase)) // Intent hayır ise
                a.ConfirmationState = ConfirmationState.Gelmeyecek; // Gelmeyecek olarak işaretliyorum
            else
                return false; // Desteklenmeyen intent ise false döndürüyorum

            _db.Appointments.Update(a);  // Güncelleme için işaretliyorum
            await _db.SaveChangesAsync(ct); // Kaydediyorum
            _log.LogInformation("Onay işlendi: AppointmentId={Id} Intent={Intent}", a.Id, intent); // Bilgi logu yazıyorum
            return true; // Başarılı sonucu döndürüyorum
        }

        /// <summary>
        /// Verilen gün için uygun slotları döner (IScheduleService'e delege eder).
        /// </summary>
        public async Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotDto>>
            GetAvailableSlotsAsync(DateOnly date, CancellationToken ct = default) // Slot listeleme metodunu uyguluyorum
        {
            var slots = await _schedule.GetDailySlotsAsync(date, ct); // Uygunluk servisinden slotları alıyorum
            return slots; // Sonucu aynen döndürüyorum
        }
    }
}
