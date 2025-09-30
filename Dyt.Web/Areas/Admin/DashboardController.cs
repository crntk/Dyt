using Microsoft.AspNetCore.Authorization; // [Authorize] için ekliyorum
using Microsoft.AspNetCore.Mvc;           // Controller için ekliyorum

namespace Dyt.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Yönetim paneli ana sayfasını sunar.
    /// </summary>
    [Area("Admin")]                   // Admin Area
    [Authorize(Roles = "Admin")]      // Sadece Admin rolündekiler erişebilir
    public class DashboardController : Controller
    {
        /// <summary>
        /// Özet metrikler ve kısayolların yer alacağı sayfa.
        /// </summary>
        public IActionResult Index()   // GET /Admin/Dashboard
        {
            return View();             // Şimdilik boş view döndürüyorum
        }
    }
}
