using Dyt.Business.Interfaces.About;
using Dyt.Contracts.About.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Sertifika/Baþarý yönetim controller'ý
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CertificateController : Controller
    {
        private readonly ICertificateService _certificate;

        public CertificateController(ICertificateService certificate)
        {
         _certificate = certificate;
   }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
  {
   var certificates = await _certificate.GetAllPublishedAsync(ct);
  return View(certificates);
    }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificateRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
      }

            try
          {
    await _certificate.CreateAsync(request, ct);
TempData["Success"] = "? Sertifika/Baþarý baþarýyla eklendi!";
    return RedirectToAction(nameof(Index));
       }
            catch (Exception ex)
{
       TempData["Error"] = $"? Hata: {ex.Message}";
     return View(request);
  }
        }

        [HttpGet]
      public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
    var cert = await _certificate.GetByIdAsync(id, ct);
          if (cert == null)
     {
          TempData["Error"] = "Sertifika bulunamadý.";
      return RedirectToAction(nameof(Index));
        }

            return View(new CertificateRequest
        {
   Title = cert.Title,
     Issuer = cert.Issuer,
     IssueDate = cert.IssueDate,
      Description = cert.Description,
ImageUrl = cert.ImageUrl,
     VerificationUrl = cert.VerificationUrl,
                DisplayOrder = cert.DisplayOrder,
       IsPublished = cert.IsPublished
        });
        }

        [HttpPost]
      [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CertificateRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
   {
        return View(request);
   }

  try
        {
      var result = await _certificate.UpdateAsync(id, request, ct);
  if (result == null)
      {
     TempData["Error"] = "Sertifika bulunamadý.";
       return RedirectToAction(nameof(Index));
   }

             TempData["Success"] = "? Sertifika/Baþarý baþarýyla güncellendi!";
                return RedirectToAction(nameof(Index));
        }
     catch (Exception ex)
 {
                TempData["Error"] = $"? Hata: {ex.Message}";
                return View(request);
            }
        }

        [HttpPost]
[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
    try
    {
    var result = await _certificate.DeleteAsync(id, ct);
      if (!result)
         {
     TempData["Error"] = "Sertifika bulunamadý.";
                }
            else
        {
      TempData["Success"] = "? Sertifika/Baþarý baþarýyla silindi!";
          }
  }
            catch (Exception ex)
            {
             TempData["Error"] = $"? Hata: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
    }
  }
}
