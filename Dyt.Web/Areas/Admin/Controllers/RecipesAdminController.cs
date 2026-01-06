using Dyt.Business.Interfaces.Recipes;
using Dyt.Contracts.Recipes.Requests;
using Dyt.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Dyt.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Admin tarafýndan tarif ekleme, düzenleme ve listeleme iþlemlerini yönetir.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RecipesAdminController : Controller
  {
        private readonly IWebHostEnvironment _env;
        private readonly IRecipeService _recipeService;
   
        public RecipesAdminController(IWebHostEnvironment env, IRecipeService recipeService)
  {
        _env = env;
     _recipeService = recipeService;
        }

    /// <summary>
        /// Admin tarif listesini gösterir
   /// </summary>
        [HttpGet]
  public async Task<IActionResult> Index(CancellationToken ct)
        {
    var request = new RecipeQueryRequest
            {
                Page = 1,
 PageSize = 100,
        IsPublished = null // Tüm tarifleri göster
  };
      
            var result = await _recipeService.QueryAsync(request, ct);
            return View(result.Items);
        }

        /// <summary>
        /// Yeni tarif ekleme formu
        /// </summary>
        [HttpGet]
     public IActionResult Create()
        {
return View();
        }

   /// <summary>
        /// Yeni tarif kaydetme
    /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        string title, 
        string summary, 
       string prepTime, 
 string calories, 
     string difficulty,
     string tags,
      string ingredients,
       string steps,
   IFormFile? image,
            CancellationToken ct)
{
       if (string.IsNullOrWhiteSpace(title))
            {
     ModelState.AddModelError("", "Tarif baþlýðý zorunludur.");
          return View();
            }
            
      string? imageUrl = null;
            
       // Fotoðraf yükleme iþlemi
            if (image != null && image.Length > 0)
 {
                imageUrl = await SaveFileAsync(image, ct);
      }

      var request = new RecipeCreateRequest
      {
   Title = title.Trim(),
        Summary = summary?.Trim() ?? "",
     ImageUrl = imageUrl,
  PrepTime = prepTime?.Trim() ?? "",
          Calories = calories?.Trim() ?? "",
         Difficulty = difficulty?.Trim() ?? "Kolay",
                Tags = tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList() ?? new List<string>(),
   Steps = steps?.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList() ?? new List<string>(),
      Ingredients = ingredients?.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList() ?? new List<string>(),
       IsPublished = true
            };

            await _recipeService.CreateAsync(request, ct);
        TempData["Success"] = "? Tarif baþarýyla eklendi!";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
   /// Tarif düzenleme formu
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
  var recipe = await _recipeService.GetByIdAsync(id, ct);
        if (recipe == null)
   {
          TempData["Error"] = "Tarif bulunamadý.";
                return RedirectToAction(nameof(Index));
         }

    return View(recipe);
        }

        /// <summary>
        /// Tarif güncelleme
        /// </summary>
        [HttpPost]
    [ValidateAntiForgeryToken]
     public async Task<IActionResult> Edit(
       int id,
            string title,
    string summary,
     string prepTime,
  string calories,
 string difficulty,
      string tags,
     string ingredients,
            string steps,
IFormFile? image,
        string? currentImageUrl,
  CancellationToken ct)
   {
       if (string.IsNullOrWhiteSpace(title))
      {
     ModelState.AddModelError("", "Tarif baþlýðý zorunludur.");
           var recipe = await _recipeService.GetByIdAsync(id, ct);
  return View(recipe);
   }
       
            string? imageUrl = currentImageUrl;
            
     // Yeni fotoðraf yüklendiyse
            if (image != null && image.Length > 0)
         {
             imageUrl = await SaveFileAsync(image, ct);
     }

    var request = new RecipeUpdateRequest
  {
       Title = title.Trim(),
                Summary = summary?.Trim() ?? "",
       ImageUrl = imageUrl,
             PrepTime = prepTime?.Trim() ?? "",
   Calories = calories?.Trim() ?? "",
    Difficulty = difficulty?.Trim() ?? "Kolay",
    Tags = tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList() ?? new List<string>(),
    Steps = steps?.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList() ?? new List<string>(),
 Ingredients = ingredients?.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToList() ?? new List<string>(),
           IsPublished = true
            };

    var success = await _recipeService.UpdateAsync(id, request, ct);
   if (success)
   {
                TempData["Success"] = "? Tarif baþarýyla güncellendi!";
  }
            else
       {
        TempData["Error"] = "Tarif güncellenirken bir hata oluþtu.";
          }

         return RedirectToAction(nameof(Index));
      }
      
        private async Task<string> SaveFileAsync(IFormFile file, CancellationToken ct)
        {
     var y = DateTime.UtcNow.Year;
   var m = DateTime.UtcNow.Month;
    var rel = Path.Combine("uploads", "recipes", y.ToString(), m.ToString("D2"));
  var dir = Path.Combine(_env.WebRootPath, rel);
  Directory.CreateDirectory(dir);
   var name = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(dir, name);
            await using (var fs = new FileStream(path, FileMode.Create))
   {
    await file.CopyToAsync(fs, ct);
            }
   var url = "/" + Path.Combine(rel, name).Replace("\\", "/");
       return url;
   }
        
        /// <summary>
        /// Tarif silme
      /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
 var success = await _recipeService.DeleteAsync(id, ct);
            if (success)
       {
     TempData["Success"] = "??? Tarif baþarýyla silindi!";
            }
       else
 {
          TempData["Error"] = "Tarif bulunamadý.";
  }
  
      return RedirectToAction(nameof(Index));
        }
 }
}
