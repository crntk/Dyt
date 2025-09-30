using Microsoft.AspNetCore.Mvc; // MVC attribute ve türleri için ekliyorum

namespace Dyt.Web.Controllers // Web katmaný controller'larý için ad alanýný tanýmlýyorum
{
    /// <summary>
    /// Genel web sayfalarýný yöneten controller.
    /// Varsayýlan giriþ noktasý, randevu oluþturma sayfasýna yönlendirir.
    /// </summary>
    public class HomeController : Controller // MVC Controller taban sýnýfýndan türetiyorum
    {
        /// <summary>
        /// Ana sayfaya gelen ziyaretçiyi randevu oluþturma sayfasýna yönlendirir.
        /// </summary>
        public IActionResult Index() // GET /Home/Index veya / isteði için action'ý tanýmlýyorum
        {
            return RedirectToAction("Create", "Appointment"); // Randevu formuna yönlendiriyorum
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
