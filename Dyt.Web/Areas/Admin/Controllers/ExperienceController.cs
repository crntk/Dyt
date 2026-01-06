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
    /// Deneyim/Özgeçmiş yönetim controller'ı
    /// </summary>
  [Area("Admin")]
 [Authorize(Roles = "Admin")]
    public class ExperienceController : Controller
    {
  private readonly IExperienceService _experience;

     public ExperienceController(IExperienceService experience)
   {
     _experience = experience;
  }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var experiences = await _experience.GetAllAsync(ct);
 return View(experiences);
     }

        [HttpGet]
   public IActionResult Create()
        {
 return View();
     }

        [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create(ExperienceRequest request, CancellationToken ct)
     {
       if (!ModelState.IsValid)
  {
    return View(request);
         }

      try
          {
await _experience.CreateAsync(request, ct);
      TempData["Success"] = "✅ Deneyim başarıyla eklendi!";
 return RedirectToAction(nameof(Index));
  }
    catch (Exception ex)
  {
      TempData["Error"] = $"❌ Hata: {ex.Message}";
  return View(request);
      }
        }

 [HttpGet]
public async Task<IActionResult> Edit(int id, CancellationToken ct)
{
    var exp = await _experience.GetByIdAsync(id, ct);
  if (exp == null)
 {
          TempData["Error"] = "Deneyim bulunamadı.";
         return RedirectToAction(nameof(Index));
            }

   return View(new ExperienceRequest
       {
      Position = exp.Position,
       Institution = exp.Institution,
         Description = exp.Description,
        StartDate = exp.StartDate,
   EndDate = exp.EndDate,
       IsCurrent = exp.IsCurrent,
    Type = exp.Type,
   DisplayOrder = exp.DisplayOrder,
IsActive = exp.IsActive
    });
   }

        [HttpPost]
[ValidateAntiForgeryToken]
   public async Task<IActionResult> Edit(int id, ExperienceRequest request, CancellationToken ct)
{
   if (!ModelState.IsValid)
    {
      return View(request);
       }

      try
       {
   var result = await _experience.UpdateAsync(id, request, ct);
       if (result == null)
   {
      TempData["Error"] = "Deneyim bulunamadı.";
    return RedirectToAction(nameof(Index));
    }

     TempData["Success"] = "✅ Deneyim başarıyla güncellendi!";
       return RedirectToAction(nameof(Index));
  }
       catch (Exception ex)
         {
    TempData["Error"] = $"❌ Hata: {ex.Message}";
   return View(request);
  }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
   {
    try
     {
  var result = await _experience.DeleteAsync(id, ct);
       if (!result)
     {
  TempData["Error"] = "Deneyim bulunamadı.";
       }
      else
    {
          TempData["Success"] = "✅ Deneyim başarıyla silindi!";
     }
}
    catch (Exception ex)
   {
      TempData["Error"] = $"❌ Hata: {ex.Message}";
  }

       return RedirectToAction(nameof(Index));
     }
    }
}
