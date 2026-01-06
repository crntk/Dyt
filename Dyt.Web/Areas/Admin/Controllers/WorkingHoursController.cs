using Microsoft.AspNetCore.Mvc;
using Dyt.Business.Interfaces.Scheduling;           // IScheduleService için ekliyorum
using Dyt.Contracts.Scheduling.Requests;            // Upsert istekleri için ekliyorum
using Microsoft.AspNetCore.Authorization;           // [Authorize] için ekliyorum
using Microsoft.AspNetCore.Mvc;                     // Controller için ekliyorum

namespace Dyt.Web.Areas.Admin.Controllers           // Admin alanı controller’ları için ad alanı
{
    /// <summary>
    /// Tarih bazlı istisna (müsaitlik) yönetim ekranlarını sunar.
    /// </summary>
    [Area("Admin")]                                  // Admin area’sına ait olduğunu belirtiyorum
    [Authorize(Roles = "Admin")]                     // Sadece Admin rolüne izin veriyorum
    public class WorkingHoursController : Controller // MVC Controller’dan türetiyorum
    {
        private readonly IScheduleService _schedule; // Servis bağımlılığını saklamak için alan tanımlıyorum

        /// <summary>
        /// Servisi DI üzerinden alarak controller’ı başlatır.
        /// </summary>
        public WorkingHoursController(IScheduleService schedule) // Kurucu metodu tanımlıyorum
        {
            _schedule = schedule; // Servisi alana atıyorum
        }

        private async Task PopulateViewDataAsync(CancellationToken ct)
        {
            // Haftalık şablon kaldırıldı, yalnızca istisnalar yüklenecek
            ViewBag.Exceptions = await _schedule.GetExceptionsAsync(null, null, ct);
            // 14 günlük önizleme
            var today = DateOnly.FromDateTime(DateTime.Today);
            var days = Enumerable.Range(0, 14).Select(i => today.AddDays(i));
            var list = new List<(DateOnly Date, string DayName, IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotDto> Slots)>();
            var closedCountMap = new Dictionary<string, int>(); // yyyy-MM-dd -> count
            foreach (var d in days)
            {
                var slots = await _schedule.GetDailySlotsAsync(d, ct);
                list.Add((d, d.DayOfWeek.ToString(), slots));

                var closed = await _schedule.GetClosedSlotStartsAsync(d, ct);
                closedCountMap[d.ToString("yyyy-MM-dd")] = closed.Count;
            }
            ViewBag.Preview = list;
            ViewBag.ClosedCountMap = closedCountMap;
        }

        /// <summary>
        /// İstisna listelerini gösteren ana sayfa.
        /// </summary>
        [HttpGet] // GET isteğini işaretliyorum
        public async Task<IActionResult> Index(CancellationToken ct) // Eylem metodunu tanımlıyorum
        {
            await PopulateViewDataAsync(ct);
            return View(); // Görünümü döndürüyorum
        }

        /// <summary>
        /// İstisna listesini kısmi görünüm olarak döner.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ExceptionsPartial(CancellationToken ct)
        {
            var exceptions = await _schedule.GetExceptionsAsync(null, null, ct);
            return PartialView("_ExceptionsTable", exceptions);
        }

        /// <summary>
        /// İstisna kaydını oluşturur/günceller.
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken] // CSRF korumasını ekliyorum
        public async Task<IActionResult> SaveException(WorkingHourExceptionUpsertRequest req, CancellationToken ct) // Upsert eylemini tanımlıyorum
        {
            if (!ModelState.IsValid)
            {
                await PopulateViewDataAsync(ct);
                return View("Index");
            }
            try
            {
                await _schedule.UpsertExceptionAsync(req, ct); // Servise delege ediyorum
                TempData["Msg"] = "İstisna kaydedildi."; // Başarılı mesajı
                return RedirectToAction(nameof(Index)); // Listeye dönüyorum
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message); // Hata mesajını ekliyorum
                await PopulateViewDataAsync(ct);
                return View("Index");
            }
        }

        /// <summary>
        /// İstisna kaydını siler.
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken] // CSRF korumasını ekliyorum
        public async Task<IActionResult> DeleteException(int id, CancellationToken ct) // Silme eylemini tanımlıyorum
        {
            await _schedule.DeleteExceptionAsync(id, ct); // Servise delege ediyorum
            TempData["Msg"] = "İstisna silindi."; // Silme mesajı
            return RedirectToAction(nameof(Index)); // Listeye dönüyorum
        }

        // Yeni: Önizlemeden slot kapatma
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseSlots([FromForm] DateOnly date, [FromForm] List<string> starts, CancellationToken ct)
        {
            var list = new List<TimeOnly>();
            foreach (var s in starts ?? new List<string>())
            {
                if (TimeOnly.TryParse(s, out var t)) list.Add(t);
            }
            var added = await _schedule.CloseSlotsAsync(date, list, ct);
            return Json(new { ok = true, added });
        }

        // Yeni: Önizlemeden slot açma
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OpenSlots([FromForm] DateOnly date, [FromForm] List<string> starts, CancellationToken ct)
        {
            var list = new List<TimeOnly>();
            foreach (var s in starts ?? new List<string>())
            {
                if (TimeOnly.TryParse(s, out var t)) list.Add(t);
            }
            var affected = await _schedule.OpenSlotsAsync(date, list, ct);
            return Json(new { ok = true, affected });
        }

        // Yeni: gün için kapalı slot başlangıçları - Route düzeltildi
        [HttpGet]
        [Route("Admin/WorkingHours/ClosedSlotStarts")]
        public async Task<IActionResult> ClosedSlotStarts([FromQuery] DateOnly date, CancellationToken ct)
        {
            var list = await _schedule.GetClosedSlotStartsAsync(date, ct);
            var arr = list.Select(t => t.ToString("HH:mm")).ToArray();
            return Json(arr);
        }

        // Yeni: gün için randevulu slot başlangıçları (onaylı) - Route düzeltildi
        [HttpGet]
        [Route("Admin/WorkingHours/ReservedSlotStarts")]
        public async Task<IActionResult> ReservedSlotStarts([FromQuery] DateOnly date, [FromQuery] bool onlyConfirmed = true, CancellationToken ct = default)
        {
            var list = await _schedule.GetReservedSlotStartsAsync(date, onlyConfirmed, ct);
            var arr = list.Select(t => t.ToString("HH:mm")).ToArray();
            return Json(arr);
        }

        // Yeni: gün için kapalı slot listesini güncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateClosedSlots([FromForm] DateOnly date, [FromForm] List<string> starts, CancellationToken ct)
        {
            var list = new List<TimeOnly>();
            foreach (var s in starts ?? new List<string>())
            {
                if (TimeOnly.TryParse(s, out var t)) list.Add(t);
            }
            var ok = await _schedule.UpdateClosedSlotsForDateAsync(date, list, ct);
            return Json(new { ok });
        }
    }
}

