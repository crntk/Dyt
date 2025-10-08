using Dyt.Business.Interfaces.Appointments;                 // Randevu servis arayüzünü kullanmak için ekliyorum
using Dyt.Contracts.Appointments.Requests;                 // Business'a gidecek DTO için ekliyorum
using Dyt.Web.ViewModels;                                  // UI ViewModel'ini kullanmak için ekliyorum
using Microsoft.AspNetCore.Mvc;                            // MVC altyapısı için ekliyorum

namespace Dyt.Web.Controllers                               // Web katmanı controller'ları için ad alanını belirtiyorum
{
    /// <summary>
    /// Kamuya açık randevu uçlarını barındıran controller.
    /// </summary>
    public class AppointmentController : Controller         // MVC Controller taban sınıfından türetiyorum
    {
        private readonly IAppointmentService _appointments; // Randevu servisini saklamak için alan tanımlıyorum

        /// <summary>
        /// Randevu servisini DI üzerinden alarak controller'ı başlatır.
        /// </summary>
        public AppointmentController(IAppointmentService appointments) // Kurucu metotta bağımlılığı alıyorum
        {
            _appointments = appointments; // Bağımlılığı alan değişkenine atıyorum
        }

        /// <summary>
        /// Randevu oluşturma formunu gösterir.
        /// </summary>
        [HttpGet] // HTTP GET isteği için attribute ekliyorum
        public IActionResult Create() // Form sayfasını dönen action'ı tanımlıyorum
        {
            return View(new AppointmentCreateVm()); // Boş ViewModel ile view'ı döndürüyorum
        }

        /// <summary>
        /// Randevu oluşturma formundan gelen verileri işleyip randevuyu oluşturur.
        /// Başarılıysa Detay sayfasına yönlendirir.
        /// </summary>
        [HttpPost] // HTTP POST isteği için attribute ekliyorum
        [ValidateAntiForgeryToken] // CSRF koruması için anti-forgery doğrulaması ekliyorum
        public async Task<IActionResult> Create(AppointmentCreateVm vm, CancellationToken ct) // Form post action'ını tanımlıyorum
        {
            if (!ModelState.IsValid) // Model doğrulaması başarısızsa
                return View(vm); // Aynı sayfayı hata mesajlarıyla geri döndürüyorum

            if (vm.Date is null || vm.StartTime is null) // Tarih ya da saat boşsa (ek koruma)
            {
                ModelState.AddModelError(string.Empty, "Lütfen tarih ve saat seçiniz."); // Genel bir hata mesajı ekliyorum
                return View(vm); // Formu geri döndürüyorum
            }

            // ViewModel'den Business katmanına gidecek DTO'yu hazırlıyorum
            var dto = new AppointmentCreateRequest // Randevu oluşturma DTO'sunu örnekliyorum
            {
                Date = vm.Date.Value, // Tarihi zorunlu alan olarak set ediyorum
                StartTime = vm.StartTime.Value, // Saat başlangıcını set ediyorum
                ClientName = vm.ClientName.Trim(), // Adı kırpıp set ediyorum
                ClientPhone = vm.ClientPhone.Trim(), // Telefonu kırpıp set ediyorum
                ClientEmail = string.IsNullOrWhiteSpace(vm.ClientEmail) ? null : vm.ClientEmail.Trim(), // E-posta boşsa null, değilse kırpıp set ediyorum
                Note = string.IsNullOrWhiteSpace(vm.Note) ? null : vm.Note.Trim(), // Not için de aynı işlemi yapıyorum
                Channel = vm.Channel
            };

            var created = await _appointments.CreateAsync(dto, ct); // Randevu oluşturmayı Business servisine delege ediyorum

            return RedirectToAction(nameof(Details), new { id = created.Id }); // Başarılıysa Detay sayfasına yönlendiriyorum
        }

        /// <summary>
        /// Oluşturulan randevunun özetini gösterir.
        /// </summary>
        [HttpGet] // HTTP GET isteği için attribute ekliyorum
        public async Task<IActionResult> Details(int id, CancellationToken ct) // Detay sayfası action'ını tanımlıyorum
        {
            var a = await _appointments.GetByIdAsync(id, ct); // Randevuyu servisten alıyorum
            if (a == null) return NotFound(); // Bulunamazsa 404 döndürüyorum
            return View(a); // DTO'yu view'a model olarak veriyorum
        }

        /// <summary>
        /// Onay/ret linkinden gelen token ve intent'i işleyip sonuç döner.
        /// GET /appointment/confirm?token=...&intent=yes|no
        /// </summary>
        [HttpGet("/appointment/confirm")] // Mutlak rota ile endpoint'i tanımlıyorum
        public async Task<IActionResult> Confirm(string token, string intent, CancellationToken ct) // Onay endpoint'ini tanımlıyorum
        {
            var ok = await _appointments.ProcessConfirmationAsync(token, intent, ct); // Onayı işliyorum
            if (!ok) return View("ConfirmResult", model: "Geçersiz veya süresi dolmuş bağlantı."); // Hatalıysa mesaj gösteriyorum

            var text = string.Equals(intent, "yes", StringComparison.OrdinalIgnoreCase) // Intent'e göre mesajı seçiyorum
                ? "Teşekkürler, randevunuz onaylandı." // Evet durumu için metin
                : "Bilgi için teşekkürler, randevunuza gelemeyeceğinizi ilettiniz."; // Hayır durumu için metin

            return View("ConfirmResult", model: text); // Sonuç sayfasını döndürüyorum
        }

        /// <summary>
        /// Verilen gün için uygun slotları JSON olarak döner. (Örn. AJAX ile kullanılır.)
        /// GET /appointment/slots?date=yyyy-MM-dd
        /// </summary>
        [HttpGet("/appointment/slots")] // Mutlak rota ile endpoint'i tanımlıyorum
        public async Task<IActionResult> Slots([FromQuery] DateOnly date, CancellationToken ct) // Slot listeleme action'ını tanımlıyorum
        {
            var slots = await _appointments.GetAvailableSlotsAsync(date, ct); // Uygun slotları alıyorum
            return Json(slots); // JSON olarak döndürüyorum
        }

        /// <summary>
        /// Verilen gün için tüm slotlar ve doluluk durumlarını JSON döner.
        /// GET /appointment/slot-states?date=yyyy-MM-dd
        /// </summary>
        [HttpGet("/appointment/slot-states")]
        public async Task<IActionResult> SlotStates([FromQuery] DateOnly date, CancellationToken ct)
        {
            var states = await _appointments.GetSlotStatesAsync(date, ct);
            return Json(states);
        }
    }
}
