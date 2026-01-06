using Dyt.Business.Interfaces.Recipes;
using Dyt.Business.Utils;
using Dyt.Contracts.Common;
using Dyt.Contracts.Recipes.Requests;
using Dyt.Contracts.Recipes.Responses;
using Dyt.Data.Context;
using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dyt.Business.Services.Recipes
{
  public class RecipeService : IRecipeService
    {
    private readonly AppDbContext _db;
    private readonly ILogger<RecipeService> _log;

        public RecipeService(AppDbContext db, ILogger<RecipeService> log)
        {
            _db = db;
  _log = log;
  }

        /// <summary>
        /// Tarifleri filtreleyip sayfalayarak döner.
        /// </summary>
        public async Task<PagedResult<RecipeDto>> QueryAsync(RecipeQueryRequest request, CancellationToken ct = default)
        {
            var q = _db.Recipes.AsNoTracking()
         .Include(r => r.Ingredients)
        .Include(r => r.Steps)
 .Include(r => r.RecipeTags)
.ThenInclude(rt => rt.Tag)
                .AsQueryable();

            // Arama filtresi
     if (!string.IsNullOrWhiteSpace(request.Search))
  {
              var s = request.Search.Trim();
                q = q.Where(r => r.Title.Contains(s) || r.Summary.Contains(s));
         }

    // Tag filtresi
            if (!string.IsNullOrWhiteSpace(request.Tag))
  {
         var tag = request.Tag.Trim();
        q = q.Where(r => r.RecipeTags.Any(rt => rt.Tag.Name == tag || rt.Tag.Slug == tag));
   }

            // Zorluk filtresi
     if (!string.IsNullOrWhiteSpace(request.Difficulty))
{
          q = q.Where(r => r.Difficulty == request.Difficulty);
   }

  // Yayýn durumu filtresi
         if (request.IsPublished.HasValue)
     {
       q = q.Where(r => r.IsPublished == request.IsPublished.Value);
   }

            var total = await q.CountAsync(ct);

        // Sýralama
            q = q.OrderByDescending(r => r.PublishDateUtc ?? r.CreatedAtUtc);

 // Sayfalama
            var page = request.Page <= 0 ? 1 : request.Page;
       var size = request.PageSize <= 0 ? 10 : Math.Min(request.PageSize, 100);
 var skip = (page - 1) * size;

            var list = await q.Skip(skip).Take(size)
        .Select(r => new RecipeDto
     {
 Id = r.Id,
  Title = r.Title,
Slug = r.Slug,
 Summary = r.Summary,
       ImageUrl = r.ImageUrl,
         PrepTime = r.PrepTime,
 Calories = r.Calories,
    Difficulty = r.Difficulty,
         IsPublished = r.IsPublished,
    PublishDateUtc = r.PublishDateUtc,
Tags = r.RecipeTags.Select(rt => rt.Tag.Name).ToList(),
     Ingredients = r.Ingredients.OrderBy(i => i.DisplayOrder)
 .Select(i => new RecipeIngredientDto
       {
      Id = i.Id,
    Name = i.Name,
     DisplayOrder = i.DisplayOrder
   }).ToList(),
  Steps = r.Steps.OrderBy(s => s.StepNumber)
 .Select(s => new RecipeStepDto
       {
   Id = s.Id,
       Description = s.Description,
   StepNumber = s.StepNumber
 }).ToList()
    })
      .ToListAsync(ct);

 // Debug için log ekle
       foreach (var item in list)
{
  _log.LogInformation("Tarif: {Title}, ImageUrl: {ImageUrl}", item.Title, item.ImageUrl ?? "NULL");
          }

 return new PagedResult<RecipeDto>
    {
  Items = list,
          TotalCount = total,
        Page = page,
             PageSize = size
            };
        }

 /// <summary>
        /// Yeni tarif oluþturur.
        /// </summary>
        public async Task<int> CreateAsync(RecipeCreateRequest request, CancellationToken ct = default)
{
   // Slug oluþtur
  var slug = GenerateSlug(request.Title);
      
          // Slug benzersiz mi kontrol et
        var slugExists = await _db.Recipes.AnyAsync(r => r.Slug == slug, ct);
       if (slugExists)
 {
    slug = $"{slug}-{Guid.NewGuid():N}".Substring(0, 220);
      }

          var recipe = new Recipe
            {
           Title = request.Title.Trim(),
  Slug = slug,
            Summary = request.Summary?.Trim() ?? string.Empty,
        ImageUrl = request.ImageUrl,
          PrepTime = request.PrepTime?.Trim() ?? string.Empty,
             Calories = request.Calories?.Trim() ?? string.Empty,
  Difficulty = request.Difficulty?.Trim() ?? "Kolay",
          IsPublished = request.IsPublished,
                PublishDateUtc = request.IsPublished ? DateTime.UtcNow : null,
            CreatedAtUtc = DateTime.UtcNow
    };

       // Malzemeleri ekle
   for (int i = 0; i < request.Ingredients.Count; i++)
     {
     recipe.Ingredients.Add(new RecipeIngredient
        {
    Name = request.Ingredients[i].Trim(),
      DisplayOrder = i,
          CreatedAtUtc = DateTime.UtcNow
     });
            }

            // Adýmlarý ekle
for (int i = 0; i < request.Steps.Count; i++)
   {
recipe.Steps.Add(new RecipeStep
          {
   Description = request.Steps[i].Trim(),
            StepNumber = i + 1,
         CreatedAtUtc = DateTime.UtcNow
         });
            }

    // Etiketleri ekle
      foreach (var tagName in request.Tags.Select(t => t.Trim()).Distinct())
            {
             var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName, ct);
       if (tag == null)
     {
 tag = new Tag
       {
             Name = tagName,
    Slug = GenerateSlug(tagName),
          CreatedAtUtc = DateTime.UtcNow
       };
        await _db.Tags.AddAsync(tag, ct);
      }

      recipe.RecipeTags.Add(new RecipeTag
        {
          Tag = tag
     });
         }

         await _db.Recipes.AddAsync(recipe, ct);
        await _db.SaveChangesAsync(ct);

 _log.LogInformation("Tarif oluþturuldu: {Id} {Title} ImageUrl: {ImageUrl}", recipe.Id, recipe.Title, recipe.ImageUrl ?? "NULL");

            return recipe.Id;
        }

     /// <summary>
/// ID'ye göre tarif döner.
        /// </summary>
        public async Task<RecipeDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var recipe = await _db.Recipes.AsNoTracking()
       .Include(r => r.Ingredients)
            .Include(r => r.Steps)
       .Include(r => r.RecipeTags)
     .ThenInclude(rt => rt.Tag)
  .FirstOrDefaultAsync(r => r.Id == id, ct);

         if (recipe == null) return null;

        return MapToDto(recipe);
     }

        /// <summary>
        /// Slug'a göre tarif döner.
        /// </summary>
        public async Task<RecipeDto?> GetBySlugAsync(string slug, CancellationToken ct = default)
        {
            var recipe = await _db.Recipes.AsNoTracking()
   .Include(r => r.Ingredients)
     .Include(r => r.Steps)
          .Include(r => r.RecipeTags)
     .ThenInclude(rt => rt.Tag)
     .FirstOrDefaultAsync(r => r.Slug == slug, ct);

       if (recipe == null) return null;

   return MapToDto(recipe);
        }

        /// <summary>
        /// Tarif günceller.
        /// </summary>
        public async Task<bool> UpdateAsync(int id, RecipeUpdateRequest request, CancellationToken ct = default)
        {
            var recipe = await _db.Recipes
    .Include(r => r.Ingredients)
    .Include(r => r.Steps)
     .Include(r => r.RecipeTags)
         .ThenInclude(rt => rt.Tag)
     .FirstOrDefaultAsync(r => r.Id == id, ct);

          if (recipe == null) return false;

    // Temel bilgileri güncelle
            recipe.Title = request.Title.Trim();
            recipe.Summary = request.Summary?.Trim() ?? string.Empty;
            recipe.ImageUrl = request.ImageUrl;
 recipe.PrepTime = request.PrepTime?.Trim() ?? string.Empty;
            recipe.Calories = request.Calories?.Trim() ?? string.Empty;
            recipe.Difficulty = request.Difficulty?.Trim() ?? "Kolay";
       recipe.IsPublished = request.IsPublished;
   recipe.UpdatedAtUtc = DateTime.UtcNow;

   if (request.IsPublished && !recipe.PublishDateUtc.HasValue)
            {
       recipe.PublishDateUtc = DateTime.UtcNow;
   }

     // Malzemeleri güncelle - eskilerini sil, yenilerini ekle
  _db.RecipeIngredients.RemoveRange(recipe.Ingredients);
       recipe.Ingredients.Clear();

for (int i = 0; i < request.Ingredients.Count; i++)
  {
      recipe.Ingredients.Add(new RecipeIngredient
     {
        Name = request.Ingredients[i].Trim(),
              DisplayOrder = i,
             CreatedAtUtc = DateTime.UtcNow
           });
     }

         // Adýmlarý güncelle
          _db.RecipeSteps.RemoveRange(recipe.Steps);
       recipe.Steps.Clear();

  for (int i = 0; i < request.Steps.Count; i++)
    {
      recipe.Steps.Add(new RecipeStep
     {
         Description = request.Steps[i].Trim(),
   StepNumber = i + 1,
   CreatedAtUtc = DateTime.UtcNow
    });
            }

  // Etiketleri güncelle
            _db.RecipeTags.RemoveRange(recipe.RecipeTags);
 recipe.RecipeTags.Clear();

            foreach (var tagName in request.Tags.Select(t => t.Trim()).Distinct())
   {
var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName, ct);
     if (tag == null)
{
  tag = new Tag
     {
     Name = tagName,
               Slug = GenerateSlug(tagName),
            CreatedAtUtc = DateTime.UtcNow
          };
        await _db.Tags.AddAsync(tag, ct);
     }

    recipe.RecipeTags.Add(new RecipeTag
         {
      Tag = tag
     });
    }

     _db.Recipes.Update(recipe);
        await _db.SaveChangesAsync(ct);

    _log.LogInformation("Tarif güncellendi: {Id} {Title} ImageUrl: {ImageUrl}", recipe.Id, recipe.Title, recipe.ImageUrl ?? "NULL");

            return true;
        }

 /// <summary>
        /// Tarif siler (soft delete).
        /// </summary>
  public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
  var recipe = await _db.Recipes.FirstOrDefaultAsync(r => r.Id == id, ct);
if (recipe == null) return false;

  _db.Recipes.Remove(recipe);
      await _db.SaveChangesAsync(ct);

            _log.LogInformation("Tarif silindi: {Id} {Title}", recipe.Id, recipe.Title);

    return true;
        }

        private static RecipeDto MapToDto(Recipe recipe)
        {
  return new RecipeDto
       {
    Id = recipe.Id,
      Title = recipe.Title,
       Slug = recipe.Slug,
      Summary = recipe.Summary,
                ImageUrl = recipe.ImageUrl,
  PrepTime = recipe.PrepTime,
           Calories = recipe.Calories,
       Difficulty = recipe.Difficulty,
              IsPublished = recipe.IsPublished,
                PublishDateUtc = recipe.PublishDateUtc,
    Tags = recipe.RecipeTags.Select(rt => rt.Tag.Name).ToList(),
      Ingredients = recipe.Ingredients.OrderBy(i => i.DisplayOrder)
           .Select(i => new RecipeIngredientDto
            {
     Id = i.Id,
         Name = i.Name,
            DisplayOrder = i.DisplayOrder
               }).ToList(),
   Steps = recipe.Steps.OrderBy(s => s.StepNumber)
         .Select(s => new RecipeStepDto
             {
       Id = s.Id,
        Description = s.Description,
           StepNumber = s.StepNumber
       }).ToList()
            };
        }

        private static string GenerateSlug(string text)
      {
        // Basit slug oluþturma
            var slug = text.ToLowerInvariant()
         .Replace("ý", "i")
                .Replace("ð", "g")
       .Replace("ü", "u")
            .Replace("þ", "s")
    .Replace("ö", "o")
      .Replace("ç", "c")
     .Replace(" ", "-")
       .Replace("'", "");

// Alfanumerik olmayan karakterleri temizle
        slug = new string(slug.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

            return slug.Length > 220 ? slug.Substring(0, 220) : slug;
        }
    }
}
