using Dyt.Business.Interfaces.About;
using Dyt.Business.Utils;
using Dyt.Contracts.About.Requests;
using Dyt.Contracts.About.Responses;
using Dyt.Data.Context;
using Dyt.Data.Entities.Content;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Business.Services.About
{
    /// <summary>
    /// "Ben Kimim" bölümü servisi
    /// </summary>
    public class AboutSectionService : IAboutSectionService
    {
   private readonly AppDbContext _db;

        public AboutSectionService(AppDbContext db)
        {
         _db = db;
  }

 public async Task<AboutSectionDto?> GetActiveAsync(CancellationToken ct = default)
     {
  var section = await _db.AboutSections
        .Where(x => !x.IsDeleted && x.IsPublished)
       .OrderByDescending(x => x.CreatedAtUtc)
    .FirstOrDefaultAsync(ct);

            if (section == null) return null;

   return new AboutSectionDto
  {
       Id = section.Id,
    Title = section.Title,
      ContentMarkdown = section.ContentMarkdown,
 ContentHtml = MarkdownHelper.ToHtml(section.ContentMarkdown),
    IsPublished = section.IsPublished,
      CreatedAtUtc = section.CreatedAtUtc,
    UpdatedAtUtc = section.UpdatedAtUtc
    };
}

        public async Task<AboutSectionDto?> GetCurrentAsync(CancellationToken ct = default)
  {
     // Admin panel için: Yayýnda olsun ya da olmasýn mevcut kaydý getir
     var section = await _db.AboutSections
     .Where(x => !x.IsDeleted)
        .OrderByDescending(x => x.CreatedAtUtc)
       .FirstOrDefaultAsync(ct);

      if (section == null) return null;

       return new AboutSectionDto
    {
  Id = section.Id,
        Title = section.Title,
    ContentMarkdown = section.ContentMarkdown,
   ContentHtml = MarkdownHelper.ToHtml(section.ContentMarkdown),
   IsPublished = section.IsPublished,
  CreatedAtUtc = section.CreatedAtUtc,
     UpdatedAtUtc = section.UpdatedAtUtc
     };
        }

        public async Task<AboutSectionDto> UpsertAsync(AboutSectionRequest request, CancellationToken ct = default)
      {
          // Mevcut kaydý bul
      var existing = await _db.AboutSections
         .Where(x => !x.IsDeleted)
      .FirstOrDefaultAsync(ct);

         if (existing != null)
      {
 // Güncelle
   existing.Title = request.Title;
     existing.ContentMarkdown = request.ContentMarkdown;
  existing.IsPublished = request.IsPublished;
   existing.UpdatedAtUtc = DateTime.UtcNow;

    await _db.SaveChangesAsync(ct);

         return new AboutSectionDto
    {
       Id = existing.Id,
     Title = existing.Title,
 ContentMarkdown = existing.ContentMarkdown,
 ContentHtml = MarkdownHelper.ToHtml(existing.ContentMarkdown),
      IsPublished = existing.IsPublished,
         CreatedAtUtc = existing.CreatedAtUtc,
      UpdatedAtUtc = existing.UpdatedAtUtc
           };
          }
  else
       {
   // Yeni oluþtur
     var newSection = new AboutSection
   {
         Title = request.Title,
  ContentMarkdown = request.ContentMarkdown,
          IsPublished = request.IsPublished,
      CreatedAtUtc = DateTime.UtcNow
      };

      _db.AboutSections.Add(newSection);
         await _db.SaveChangesAsync(ct);

       return new AboutSectionDto
      {
     Id = newSection.Id,
Title = newSection.Title,
  ContentMarkdown = newSection.ContentMarkdown,
  ContentHtml = MarkdownHelper.ToHtml(newSection.ContentMarkdown),
IsPublished = newSection.IsPublished,
    CreatedAtUtc = newSection.CreatedAtUtc,
  UpdatedAtUtc = newSection.UpdatedAtUtc
        };
       }
     }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
     {
            var section = await _db.AboutSections.FindAsync(new object[] { id }, ct);
 if (section == null || section.IsDeleted) return false;

    section.IsDeleted = true;
            section.DeletedAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
     return true;
   }
    }
}
