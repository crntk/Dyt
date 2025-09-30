using Dyt.Data.Entities.Identity;         // AppUser için ekliyorum
using Microsoft.AspNetCore.Authorization; // [AllowAnonymous] için ekliyorum
using Microsoft.AspNetCore.Identity;      // SignInManager için ekliyorum
using Microsoft.AspNetCore.Mvc;           // Controller için ekliyorum

namespace Dyt.Web.Controllers
{
    /// <summary>
    /// Giriş/çıkış işlemlerini yöneten controller.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signIn; // Oturum açma/yönetimi için alan tanımlıyorum

        /// <summary>
        /// Gerekli bağımlılıkları alır.
        /// </summary>
        public AccountController(SignInManager<AppUser> signIn) // Kurucu
        {
            _signIn = signIn; // DI'dan gelen SignInManager'ı saklıyorum
        }

        /// <summary>
        /// Giriş formunu gösterir.
        /// </summary>
        [HttpGet, AllowAnonymous] // Kimliği olmayan da görebilsin istiyorum
        public IActionResult Login(string? returnUrl = null) // İsteğe bağlı dönüş adresi alıyorum
        {
            ViewData["ReturnUrl"] = returnUrl; // View tarafına taşıyorum
            return View(); // Giriş sayfasını döndürüyorum
        }

        /// <summary>
        /// Giriş formu gönderildiğinde kimlik doğrulama yapar.
        /// </summary>
        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken] // CSRF koruması
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null) // Basit model: e-posta ve parola
        {
            ViewData["ReturnUrl"] = returnUrl;                          // Dönüş adresini view'a taşıyorum
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) // Basit doğrulama
            {
                ModelState.AddModelError(string.Empty, "E-posta ve parola zorunludur."); // Hata mesajı
                return View(); // Formu geri döndürüyorum
            }

            var result = await _signIn.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false); // Oturum açmayı deniyorum
            if (!result.Succeeded) // Başarısızsa
            {
                ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi."); // Hata ekliyorum
                return View(); // Yeniden gösteriyorum
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)) // Dönüş adresi güvenliyse
                return Redirect(returnUrl); // Oraya dönüyorum

            return RedirectToAction("Index", "Dashboard", new { area = "Admin" }); // Admin paneline yönlendiriyorum
        }

        /// <summary>
        /// Oturumu kapatır.
        /// </summary>
        [HttpPost, ValidateAntiForgeryToken] // CSRF koruması
        public async Task<IActionResult> Logout() // Çıkış action'ı
        {
            await _signIn.SignOutAsync(); // Oturumu kapatıyorum
            return RedirectToAction("Login", "Account"); // Giriş sayfasına dönüyorum
        }

        /// <summary>
        /// Yetki olmadığında gösterilecek basit sayfa.
        /// </summary>
        [HttpGet, AllowAnonymous] // Anonymous erişilebilir
        public IActionResult Denied() // Erişim engellendi sayfası
        {
            return Content("Erişim engellendi."); // Basit metin döndürüyorum (istersen view yaparız)
        }
    }
}
