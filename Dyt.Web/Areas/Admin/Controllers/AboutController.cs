using Dyt.Business.Interfaces.About;
using Dyt.Business.Utils;
using Dyt.Contracts.About.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// "Ben Kimim" bölümü yönetim controller'ý
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AboutController : Controller
    {
        private readonly IAboutSectionService _aboutSection;

        public AboutController(IAboutSectionService aboutSection)
        {
            _aboutSection = aboutSection;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var section = await _aboutSection.GetCurrentAsync(ct);
            return View(section);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(AboutSectionRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm gerekli alanlarý doldurun.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _aboutSection.UpsertAsync(request, ct);
                TempData["Success"] = "? Ben Kimim bölümü baþarýyla kaydedildi!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"? Hata: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Markdown içeriðini HTML'e çeviren önizleme endpoint'i
        /// </summary>
        [HttpPost]
        [IgnoreAntiforgeryToken] // Önizleme için CSRF token kontrolünü devre dýþý býrak
        public IActionResult PreviewMarkdown([FromBody] MarkdownPreviewRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Markdown))
            {
                return Json(new { html = "" });
            }

            var html = MarkdownHelper.ToHtml(request.Markdown);
            return Json(new { html });
        }
    }

    /// <summary>
    /// Markdown önizleme için request model
    /// </summary>
    public class MarkdownPreviewRequest
    {
        public string Markdown { get; set; } = string.Empty;
    }
}
