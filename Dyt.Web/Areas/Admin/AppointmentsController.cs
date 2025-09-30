using Dyt.Business.Interfaces.Appointments; // Randevu servisini kullanmak için ekliyorum
using Dyt.Contracts.Appointments.Requests; // Sorgu isteği modelini kullanmak için ekliyorum
using Microsoft.AspNetCore.Authorization; // Admin erişim kontrolü için ekliyorum
using Microsoft.AspNetCore.Mvc; // MVC için ekliyorum

namespace Dyt.Web.Areas.Admin.Controllers // Admin Area controller'ları için ad alanını tanımlıyorum
{
    /// <summary>
    /// Admin tarafında randevu listeleme ve filtreleme ekranını sunan controller.
    /// </summary>
    [Area("Admin")] // Bu controller'ın Admin alanına ait olduğunu belirtiyorum
    [Authorize] // Yönetici girişi gerektiren erişim kontrolü ekliyorum (rol bazlı kısıtlama ekleyebiliriz)
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
    }
}
