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
        /// Ana sayfa
        /// </summary>
        public async Task<IActionResult> Index(CancellationToken ct) // GET /Home/Index veya / isteði için action'ý tanýmlýyorum
        {
            var now = DateTime.UtcNow;
            
            // Son 6 blog yazýsý
            var latestBlogs = await _db.BlogPosts.AsNoTracking()
                .Where(p => p.IsPublished && (p.PublishDateUtc == null || p.PublishDateUtc <= now))
                .Include(p => p.Media).ThenInclude(m => m.MediaFile)
                .OrderByDescending(p => p.PublishDateUtc ?? p.CreatedAtUtc)
                .Take(6)
                .ToListAsync(ct);

            // Son 3 tarif - EN SON EKLENEN ÖNCELÝKLÝ
            var latestRecipes = await _db.Recipes.AsNoTracking()
                .Where(r => r.IsPublished && (r.PublishDateUtc == null || r.PublishDateUtc <= now))
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.RecipeTags).ThenInclude(rt => rt.Tag)
                .OrderByDescending(r => r.CreatedAtUtc)
                .Take(3)
                .ToListAsync(ct);

            ViewBag.LatestRecipes = latestRecipes;
            ViewData["FvEnable"] = true;

            return View(latestBlogs);
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
