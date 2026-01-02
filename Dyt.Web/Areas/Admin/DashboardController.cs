using Microsoft.AspNetCore.Authorization; // [Authorize] için ekliyorum
using Microsoft.AspNetCore.Mvc;    // Controller için ekliyorum
using Dyt.Business.Interfaces.Appointments; // Randevu servisi için
using Dyt.Data.Context; // AppDbContext için
using Microsoft.EntityFrameworkCore;      // LINQ sorguları için

namespace Dyt.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Yönetim paneli ana sayfasını sunar.
    /// </summary>
  [Area("Admin")]        // Admin Area
    [Authorize(Roles = "Admin")]      // Sadece Admin rolündekiler erişebilir
    public class DashboardController : Controller
    {
        private readonly IAppointmentService _appointments;
     private readonly AppDbContext _db;

        public DashboardController(IAppointmentService appointments, AppDbContext db)
    {
 _appointments = appointments;
    _db = db;
        }

  /// <summary>
        /// Özet metrikler ve kısayolların yer alacağı sayfa.
  /// </summary>
        public async Task<IActionResult> Index(CancellationToken ct)   // GET /Admin/Dashboard
    {
// Randevu istatistikleri
            var totalAppointments = await _db.Appointments
.Where(a => !a.IsDeleted)
    .CountAsync(ct);

     var approvedAppointments = await _db.Appointments
  .Where(a => !a.IsDeleted && a.ConfirmationState == Data.Enums.ConfirmationState.Gelecek)
         .CountAsync(ct);

  var pendingAppointments = await _db.Appointments
        .Where(a => !a.IsDeleted && a.ConfirmationState == Data.Enums.ConfirmationState.Yanıtlanmadı)
 .CountAsync(ct);

          // Blog istatistikleri
    var totalBlogPosts = await _db.BlogPosts
            .Where(b => !b.IsDeleted && b.IsPublished)
    .CountAsync(ct);

  ViewBag.TotalAppointments = totalAppointments;
    ViewBag.ApprovedAppointments = approvedAppointments;
    ViewBag.PendingAppointments = pendingAppointments;
            ViewBag.TotalBlogPosts = totalBlogPosts;

      return View();   
        }
    }
}
