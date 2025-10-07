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
            [Required, StringLength(80)]
            public string Name { get; set; } = string.Empty;
            [Required, EmailAddress, StringLength(160)]
            public string Email { get; set; } = string.Empty;
            [Required, StringLength(120)]
            public string Subject { get; set; } = string.Empty;
            [Required, StringLength(2000)]
            public string Message { get; set; } = string.Empty;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new ContactForm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactForm model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Alýcý e-posta: appsettings: Admin:Email veya Contact:ToEmail
            var to = _cfg["Contact:ToEmail"] ?? _cfg["Admin:Email"] ?? "admin@dyt.local";
            var subject = $"[Ýletiþim] {model.Subject}";
            var body = $"<p><b>Gönderen:</b> {model.Name} &lt;{model.Email}&gt;</p><p>{System.Net.WebUtility.HtmlEncode(model.Message).Replace("\n","<br/>")}</p>";

            await _email.SendAsync(to, subject, body, ct);
            TempData["Msg"] = "Mesajýnýz gönderildi. En kýsa sürede dönüþ yapýlacaktýr.";
            return RedirectToAction(nameof(Index));
        }
    }
}
