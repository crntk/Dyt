using Dyt.Business.Interfaces.Appointments; // Randevu servisini kullanmak için ekliyorum
using Dyt.Contracts.Appointments.Requests; // Sorgu isteği modelini kullanmak için ekliyorum
using Microsoft.AspNetCore.Authorization; // Admin erişim kontrolü için ekliyorum
using Microsoft.AspNetCore.Mvc; // MVC için ekliyorum
using Dyt.Data.Enums; // ConfirmationState enum
using Dyt.Contracts.Appointments.Responses; // AppointmentDto için

namespace Dyt.Web.Areas.Admin.Controllers // Admin Area controller'ları için ad alanını tanımlıyorum
{
    /// <summary>
    /// Admin tarafında randevu listeleme ve filtreleme ekranını sunan controller.
    /// </summary>
    [Area("Admin")] // Bu controller'ın Admin alanına ait olduğunu belirtiyorum
    [Authorize(Roles = "Admin")] // Yönetici girişi gerektiren erişim kontrolü ve rol kısıtı
    public class AppointmentsController : Controller // MVC Controller taban sınıfından türetiyorum
    {
        private readonly IAppointmentService _appointments; // Randevu servis bağımlılığını tutmak için alan tanımlıyorum

        /// <summary>
        /// Randevu servisini DI üzerinden alarak controller'ı başlatır.
        /// </summary>
        public AppointmentsController(IAppointmentService appointments) // Kurucu metotta bağımlılığı alıyorum
        {
            _appointments = appointments; // Bağımlılığı alan değişkenine atıyorum
        }

        /// <summary>
        /// Filtre/sayfalama ile randevu listesini gösterir.
        /// </summary>
        [HttpGet] // HTTP GET için action'ı işaretliyorum
        public async Task<IActionResult> Index( // Ekrana veri dönecek action'ı tanımlıyorum
            int page = 1, // Sayfa numarasını parametre olarak alıyorum (varsayılan 1)
            int pageSize = 10, // Sayfa boyutunu parametre olarak alıyorum (varsayılan 10)
            DateOnly? from = null, // Başlangıç tarih filtresini alıyorum
            DateOnly? to = null, // Bitiş tarih filtresini alıyorum
            string? status = null, // Durum filtresini alıyorum
            string? confirmation = null, // Onay durumu filtresini alıyorum
            string? search = null, // Arama metnini alıyorum
            CancellationToken ct = default) // İptal belirtecini alıyorum
        {
            var req = new AppointmentQueryRequest // Business katmanına gidecek sorgu isteğini oluşturuyorum
            {
                Page = page, // Sayfayı set ediyorum
                PageSize = pageSize, // Boyutu set ediyorum
                DateFrom = from, // Başlangıcı set ediyorum
                DateTo = to, // Bitişi set ediyorum
                Status = string.IsNullOrWhiteSpace(status) ? null : status, // Durum filtresini temizleyip set ediyorum
                ConfirmationState = string.IsNullOrWhiteSpace(confirmation) ? null : confirmation, // Onay filtresini set ediyorum
                Search = string.IsNullOrWhiteSpace(search) ? null : search // Arama metnini set ediyorum
            };

            var result = await _appointments.QueryAsync(req, ct); // Sorguyu business servise delege ediyorum
            ViewBag.Filters = req; // View'da formu doldurmak için filtreleri saklıyorum
            return View(result); // PagedResult<AppointmentDto> modelini view'a veriyorum
        }

        /// <summary>
        /// Admin onay/red işlemini gerçekleştirir.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetConfirmation(int id, string state, string? returnUrl = null, CancellationToken ct = default)
        {
            if (!Enum.TryParse<ConfirmationState>(state, out var s))
            {
                TempData["Msg"] = "Geçersiz onay durumu.";
                return string.IsNullOrEmpty(returnUrl) ? RedirectToAction(nameof(Index)) : Redirect(returnUrl);
            }
            var ok = await _appointments.AdminSetConfirmationAsync(id, s, ct);

            if (ok && s == ConfirmationState.Gelmeyecek)
            {
                // Reddedilen danışan adını popup için sakla
                var appt = await _appointments.GetByIdAsync(id, ct);
                if (appt != null)
                {
                    TempData["RejectInfo"] = appt.ClientName;
                }
            }

            TempData["Msg"] = ok ? "Güncellendi." : "Güncellenemedi.";
            return string.IsNullOrEmpty(returnUrl) ? RedirectToAction(nameof(Index)) : Redirect(returnUrl);
        }

        /// <summary>
        /// Admin randevu oluşturma sayfasını gösterir.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Admin tarafından randevu oluşturur.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            DateOnly date,
            TimeOnly startTime,
            string clientName,
            string clientPhone,
            string? clientEmail,
            string? channel,
            string? note,
            CancellationToken ct)
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(clientName) || clientName.Trim().Length < 3)
            {
                TempData["Error"] = "Lütfen geçerli bir ad soyad girin (en az 3 karakter).";
                return View();
            }

            if (string.IsNullOrWhiteSpace(clientPhone))
            {
                TempData["Error"] = "Lütfen telefon numarası girin.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(channel))
            {
                TempData["Error"] = "Lütfen görüşme türünü seçin.";
                return View();
            }

            // Randevu oluşturma request'i hazırla
            var request = new AppointmentCreateRequest
            {
                Date = date,
                StartTime = startTime,
                ClientName = clientName.Trim(),
                ClientPhone = clientPhone.Trim(),
                ClientEmail = clientEmail?.Trim(),
                Note = note?.Trim(),
                Channel = channel
            };

            try
            {
                var result = await _appointments.CreateAsync(request, ct);

                if (result != null)
                {
                    TempData["Success"] = "✅ Randevu başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Error"] = "Randevu oluşturulamadı.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Bir hata oluştu: {ex.Message}";
                return View();
            }
        }
    }
}
