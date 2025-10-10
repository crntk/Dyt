using Dyt.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dyt.Web.Controllers
{
    /// <summary>
    /// Genel web sayfalarýný yöneten controller.
    /// Varsayýlan giriþ noktasý, randevu oluþturma sayfasýna yönlendirir.
    /// </summary>
    public class HomeController : Controller // MVC Controller taban sýnýfýndan türetiyorum
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Ana sayfaya gelen ziyaretçiyi randevu oluþturma sayfasýna yönlendirir.
        /// </summary>
        public async Task<IActionResult> Index(CancellationToken ct) // GET /Home/Index veya / isteði için action'ý tanýmlýyorum
        {
            var now = DateTime.UtcNow;
            var latest = await _db.BlogPosts.AsNoTracking()
                .Where(p => p.IsPublished && (p.PublishDateUtc == null || p.PublishDateUtc <= now))
                .Include(p => p.Media).ThenInclude(m => m.MediaFile)
                .OrderByDescending(p => p.PublishDateUtc ?? p.CreatedAtUtc)
                .Take(6)
                .ToListAsync(ct);
            return View(latest);
        }

        /// <summary>
        /// Hakkýmda sayfasý
        /// </summary>
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Gizlilik sayfasýný gösterir (þimdilik þablon).
        /// </summary>
        public IActionResult Privacy() // GET /Home/Privacy action'ýný tanýmlýyorum
        {
            return View(); // Varsayýlan Privacy view'ýný döndürüyorum
        }
    }
}
