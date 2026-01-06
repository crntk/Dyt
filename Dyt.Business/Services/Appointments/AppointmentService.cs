using Dyt.Business.Interfaces.Appointments; // Arayüzleri kullanmak için ekliyorum
using Dyt.Business.Interfaces.Scheduling;   // Uygunluk kontrolü için ekliyorum
using Dyt.Business.Utils;                   // Zaman sağlayıcısı için ekliyorum
using Dyt.Data.Context;                     // DbContext'e erişmek için ekliyorum
using Dyt.Data.Entities.Scheduling;         // Appointment varlığı için ekliyorum
using Dyt.Data.Enums;                       // Durum ve onay enumları için ekliyorum
using Microsoft.EntityFrameworkCore;        // EF Core işlemleri için ekliyorum
using Microsoft.Extensions.Logging;         // Günlükleme için ekliyorum
using Dyt.Contracts.Common; // PagedResult<T> tipi için gerekli

namespace Dyt.Business.Services.Appointments // Servis implementasyonlarının ad alanını tanımlıyorum
{
    public class AppointmentService : IAppointmentService // Arayüzü uygulayan sınıfı bildiriyorum
    {
        private readonly AppDbContext _db;                   // Veritabanı bağlamını saklamak için alan tanımlıyorum
        private readonly IScheduleService _schedule;         // Uygunluk servisini saklamak için alan tanımlıyorum
        private readonly IConfirmationTokenService _tokens;  // Onay token servisini saklamak için alan tanımlıyorum
        private readonly IDateTimeProvider _clock;           // Zaman sağlayıcısını saklamak için alan tanımlıyorum
        private readonly ILogger<AppointmentService> _log;   // Günlükleme için logger alanı tanımlıyorum

        public AppointmentService(AppDbContext db, IScheduleService schedule, IConfirmationTokenService tokens, IDateTimeProvider clock, ILogger<AppointmentService> log) // Kurucu metodu tanımlıyorum
        {
            _db = db;           // DbContext bağımlılığını alana atıyorum
            _schedule = schedule; // Uygunluk servisini alana atıyorum
            _tokens = tokens;   // Token servisini alana atıyorum
            _clock = clock;     // Zaman sağlayıcısını alana atıyorum
            _log = log;         // Logger'ı alana atıyorum
        }

        private static string MapStatus(AppointmentStatus s) => s switch
        {
            AppointmentStatus.Scheduled => "Planlandı",
            AppointmentStatus.Canceled => "İptal",
            AppointmentStatus.NoShow => "Gelmedi",
            _ => s.ToString()
        };
        private static string MapConfirmation(ConfirmationState c) => c switch
        {
            ConfirmationState.Yanıtlanmadı => "Yanıtlanmadı",
            ConfirmationState.Gelecek => "Gelecek",
            ConfirmationState.Gelmeyecek => "Gelmeyecek",
            _ => c.ToString()
        };

        // Yardımcı: string -> enum dönüştürme (TR/EN değerleri destekler)
        private static AppointmentStatus? ParseStatus(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var s = input.Trim();
            if (string.Equals(s, "Scheduled", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Planlandı", StringComparison.OrdinalIgnoreCase))
                return AppointmentStatus.Scheduled;
            if (string.Equals(s, "Canceled", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "İptal", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Iptal", StringComparison.OrdinalIgnoreCase))
                return AppointmentStatus.Canceled;
            if (string.Equals(s, "NoShow", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Gelmedi", StringComparison.OrdinalIgnoreCase))
                return AppointmentStatus.NoShow;
            return null;
        }
        private static ConfirmationState? ParseConfirmation(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var s = input.Trim();
            if (string.Equals(s, "Yanıtlanmadı", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Yanitlanmadi", StringComparison.OrdinalIgnoreCase))
                return ConfirmationState.Yanıtlanmadı;
            if (string.Equals(s, "Gelecek", StringComparison.OrdinalIgnoreCase))
                return ConfirmationState.Gelecek;
            if (string.Equals(s, "Gelmeyecek", StringComparison.OrdinalIgnoreCase))
                return ConfirmationState.Gelmeyecek;
            if (string.Equals(s, "Pending", StringComparison.OrdinalIgnoreCase)) return ConfirmationState.Yanıtlanmadı;
            if (string.Equals(s, "Yes", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Accepted", StringComparison.OrdinalIgnoreCase)) return ConfirmationState.Gelecek;
            if (string.Equals(s, "No", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Declined", StringComparison.OrdinalIgnoreCase)) return ConfirmationState.Gelmeyecek;
            return null;
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
                ConfirmationState = ConfirmationState.Yanıtlanmadı, // Onay durumunu yanıtlanmadı yapıyorum
                Channel = request.Channel?.Trim() ?? string.Empty
            };

            await _db.Appointments.AddAsync(entity, ct); // Kaydı bağlama ekliyorum
            await _db.SaveChangesAsync(ct);              // Veritabanına yazıyorum

            _log.LogInformation("Randevu oluşturuldu: {Id} {Date} {Start} Channel={Channel}", entity.Id, entity.AppointmentDate, entity.StartTime, entity.Channel); // Bilgi logu yazıyorum

            return new Dyt.Contracts.Appointments.Responses.AppointmentDto // DTO hazırlayıp döndürüyorum
            {
                Id = entity.Id,       // Kimliği set ediyorum
                Date = entity.AppointmentDate,          // Tarihi set ediyorum
                StartTime = entity.StartTime,// Başlangıcı set ediyorum
                EndTime = entity.EndTime,   // Bitişi set ediyorum
                ClientName = entity.ClientName,         // İsim set ediyorum
                ClientPhone = entity.ClientPhone,       // Telefon set ediyorum
                ClientEmail = entity.ClientEmail,     // E-posta set ediyorum
                Status = MapStatus(entity.Status),      // Durumu metin olarak set ediyorum
                ConfirmationState = MapConfirmation(entity.ConfirmationState), // Onay durumunu metin olarak set ediyorum
                Channel = entity.Channel,
                CreatedAtUtc = entity.CreatedAtUtc
            };
        }

        /// <summary>
        /// Kimliğe göre randevu döner; bulunamazsa null döner.
        /// </summary>
        public async Task<Dyt.Contracts.Appointments.Responses.AppointmentDto?>
            GetByIdAsync(int id, CancellationToken ct = default) // Get metodunu uygulıyorum
        {
            var a = await _db.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct); // Randevuyu çekiyorum
            if (a == null) return null; // Bulunamazsa null döndürüyorum

            return new Dyt.Contracts.Appointments.Responses.AppointmentDto // DTO'ya projekte edip döndürüyorum
            {
                Id = a.Id,                          // Kimliği set ediyorum
                Date = a.AppointmentDate,          // Tarihi set ediyorum
                StartTime = a.StartTime,            // Başlangıcı set ediyorum
                EndTime = a.EndTime,                // Bitişi set ediyorum
                ClientName = a.ClientName,          // İsim set ediyorum
                ClientPhone = a.ClientPhone,        // Telefon set ediyorum
                ClientEmail = a.ClientEmail,        // E-posta set ediyorum
                Status = MapStatus(a.Status),       // Durumu set ediyorum
                ConfirmationState = MapConfirmation(a.ConfirmationState), // Onay durumunu set ediyorum
                Channel = a.Channel,
                CreatedAtUtc = a.CreatedAtUtc
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
        public async Task<bool> CancelAsync(int id, CancellationToken ct = default) // İptal metodunu uygulıyorum
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
        public async Task<bool> ProcessConfirmationAsync(string token, string intent, CancellationToken ct = default) // Onay işleme metodunu uygulıyorum
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
        public async Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotDto>> GetAvailableSlotsAsync(DateOnly date, CancellationToken ct = default) // Slot listeleme metodunu uygulıyorum
        {
            var slots = await _schedule.GetDailySlotsAsync(date, ct); // Uygunluk servisinden slotları alıyorum
            return slots; // Sonucu aynen döndürüyorum
        }

        public async Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotStateDto>> GetSlotStatesAsync(DateOnly date, CancellationToken ct = default)
        {
            return await _schedule.GetDailySlotStatesAsync(date, ct);
        }

        /// <summary>
        /// Admin tarafında randevuları filtreleyip sayfalayarak döner.
        /// </summary>
        public async Task<PagedResult<Dyt.Contracts.Appointments.Responses.AppointmentDto>>
            QueryAsync(Dyt.Contracts.Appointments.Requests.AppointmentQueryRequest request, CancellationToken ct = default) // Sayfalı sorgu metodunu tanımlıyorum
        {
            // Temel sorguyu başlatıyorum
            var q = _db.Appointments.AsNoTracking().AsQueryable(); // EF Core sorgusunu değişiklik takibi olmadan başlatıyorum

            // Tarih aralığı filtresi varsa uyguluyorum
            if (request.DateFrom.HasValue) // Başlangıç tarihi verilmiş mi kontrol ediyorum
                q = q.Where(a => a.AppointmentDate >= request.DateFrom.Value); // Başlangıç tarihine göre filtreliyorum
            if (request.DateTo.HasValue) // Bitiş tarihi verilmiş mi kontrol ediyorum
                q = q.Where(a => a.AppointmentDate <= request.DateTo.Value); // Bitiş tarihine göre filtreliyorum

            // Durum filtresi (TR/EN destekli)
            var st = ParseStatus(request.Status);
            if (st.HasValue)
                q = q.Where(a => a.Status == st.Value);

            // Onay durumu filtresi (TR/EN destekli)
            var cf = ParseConfirmation(request.ConfirmationState);
            if (cf.HasValue)
                q = q.Where(a => a.ConfirmationState == cf.Value);

            // Arama filtresi (isim veya telefon)
            if (!string.IsNullOrWhiteSpace(request.Search)) // Arama ifadesi boş değilse
            {
                var s = request.Search.Trim(); // Boşlukları kırpıyorum
                q = q.Where(a => a.ClientName.Contains(s) || a.ClientPhone.Contains(s)); // İsim veya telefonda geçenleri filtreliyorum
            }

            // Toplam kayıt sayısını alıyorum (sayfalama için)
            var total = await q.CountAsync(ct); // Toplam adedi sayıyorum

            // Varsayılan sıralama: en yeni randevu en üstte olacak şekilde tarih/saat (ters sıralı)
            q = q.OrderByDescending(a => a.AppointmentDate).ThenByDescending(a => a.StartTime); // Tarih ve saat bazlı ters sıralama yapıyorum

            // Sayfalama hesapları
            var page = request.Page <= 0 ? 1 : request.Page; // Sayfa 1'den küçükse 1 yapıyorum
            var size = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100); // Boyut 1'den küçükse 10, 100'den büyükse 100 yapıyorum
            var skip = (page - 1) * size; // Atlanacak kayıt sayısını hesaplıyorum

            // İlgili sayfadaki kayıtları çekip DTO'ya projekte ediyorum
            var list = await q.Skip(skip).Take(size)
                .Select(a => new Dyt.Contracts.Appointments.Responses.AppointmentDto // DTO'ya yansıtıyorum
                {
                    Id = a.Id, // Kimlik
                    Date = a.AppointmentDate, // Tarih
                    StartTime = a.StartTime, // Başlangıç
                    EndTime = a.EndTime, // Bitiş
                    ClientName = a.ClientName, // İsim
                    ClientPhone = a.ClientPhone, // Telefon
                    ClientEmail = a.ClientEmail, // E-posta
                    Status = MapStatus(a.Status), // Durum
                    ConfirmationState = MapConfirmation(a.ConfirmationState), // Onay durumu
                    Channel = a.Channel,
                    CreatedAtUtc = a.CreatedAtUtc
                })
                .ToListAsync(ct); // Listeye çeviriyorum

            // PagedResult<T> ile paketi hazırlayıp döndürüyorum
            return new PagedResult<Dyt.Contracts.Appointments.Responses.AppointmentDto>
            {
                Items = list, // Sayfadaki kayıtlar
                TotalCount = total, // Toplam adet
                Page = page, // Mevcut sayfa
                PageSize = size // Sayfa boyutu
            };
        }

        // Yeni: Admin onay/red ve bekleyen sayısı
        /// <summary>
        /// Admin onay/red işlemi yapar.
        /// </summary>
        public async Task<bool> AdminSetConfirmationAsync(int id, ConfirmationState state, CancellationToken ct = default)
        {
            var a = await _db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct); // Kaydı buluyorum
            if (a == null) return false; // Yoksa false döndürüyorum
            if (a.Status != AppointmentStatus.Scheduled) return false; // Planlı değilse değiştirmiyorum
            a.ConfirmationState = state; // Onay durumunu güncelliyorum
            _db.Appointments.Update(a);          // Güncelleme işaretliyorum
            await _db.SaveChangesAsync(ct);        // Kaydediyorum
            return true;                            // Başarılı sonucu döndürüyorum
        }

        /// <summary>
        /// Bekleyen randevu sayısını döner.
        /// </summary>
        public async Task<int> GetPendingCountAsync(CancellationToken ct = default)
        {
            return await _db.Appointments.AsNoTracking().CountAsync(a => a.Status == AppointmentStatus.Scheduled && a.ConfirmationState == ConfirmationState.Yanıtlanmadı, ct); // Sayıyorum
        }
    }
}
