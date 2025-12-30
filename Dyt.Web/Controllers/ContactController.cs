using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dyt.Business.Interfaces.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace Dyt.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailSender _email;
        private readonly IConfiguration _cfg;
        public ContactController(IEmailSender email, IConfiguration cfg)
        {
            _email = email;
            _cfg = cfg;
        }

        public class ContactForm
        {
            [Required(ErrorMessage = "Ad Soyad alaný zorunludur.")]
            [StringLength(80, ErrorMessage = "Ad Soyad en fazla 80 karakter olabilir.")]
            [Display(Name = "Ad Soyad")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "E-posta adresi zorunludur.")]
            [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
            [StringLength(160, ErrorMessage = "E-posta adresi en fazla 160 karakter olabilir.")]
            [Display(Name = "E-posta")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Telefon numarasý zorunludur.")]
            [RegularExpression(@"^[0-9+\s\-\(\)]+$", ErrorMessage = "Telefon numarasý sadece rakam, +, -, (, ), ve boþluk karakterleri içerebilir.")]
            [StringLength(20, MinimumLength = 10, ErrorMessage = "Telefon numarasý en az 10, en fazla 20 karakter olmalýdýr.")]
            [Display(Name = "Telefon")]
            public string Phone { get; set; } = string.Empty;

            [Required(ErrorMessage = "Konu alaný zorunludur.")]
            [StringLength(120, ErrorMessage = "Konu en fazla 120 karakter olabilir.")]
            [Display(Name = "Konu")]
            public string Subject { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mesajýnýz alaný zorunludur.")]
            [StringLength(2000, MinimumLength = 10, ErrorMessage = "Mesajýnýz en az 10, en fazla 2000 karakter olmalýdýr.")]
          [Display(Name = "Mesajýnýz")]
            public string Message { get; set; } = string.Empty;

            public bool KvkkConsent { get; set; }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactForm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactForm model, CancellationToken ct)
        {
            // KVKK consent kontrolü
   if (!model.KvkkConsent)
   {
         ModelState.AddModelError(nameof(model.KvkkConsent), "KVKK aydýnlatma metnini kabul etmelisiniz.");
         }

  if (!ModelState.IsValid)
    return View(model);

            // Alýcý e-posta: appsettings: Admin:Email veya Contact:ToEmail
            var to = _cfg["Contact:ToEmail"] ?? _cfg["Admin:Email"] ?? "admin@dyt.local";
            var subject = $"[Ýletiþim] {model.Subject}";
     var body = $"<p><b>Gönderen:</b> {model.Name} &lt;{model.Email}&gt;</p><p><b>Telefon:</b> {model.Phone}</p><p><b>Mesaj:</b><br/>{System.Net.WebUtility.HtmlEncode(model.Message).Replace("\n","<br/>")}</p>";

       await _email.SendAsync(to, subject, body, ct);
        TempData["Msg"] = "Mesajýnýz gönderildi. En kýsa sürede dönüþ yapýlacaktýr.";
     return RedirectToAction(nameof(Index));
      }
    }
}
