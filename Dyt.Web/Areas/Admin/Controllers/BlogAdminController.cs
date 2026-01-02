using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dyt.Data.Context;
using Dyt.Data.Entities.Content;
using Dyt.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dyt.Business.Interfaces.Notifications;
using Microsoft.Extensions.Logging;

namespace Dyt.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BlogAdminController : Controller
    {
  private readonly AppDbContext _db;
   private readonly IWebHostEnvironment _env;
        private readonly INewsletterNotificationService _newsletterService;
  private readonly ILogger<BlogAdminController> _logger;

        public BlogAdminController(
      AppDbContext db, 
  IWebHostEnvironment env, 
        INewsletterNotificationService newsletterService,
            ILogger<BlogAdminController> logger)
        {
            _db = db;
   _env = env;
          _newsletterService = newsletterService;
    _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? open, CancellationToken ct)
        {
         var latest = await _db.BlogPosts.AsNoTracking()
     .OrderByDescending(x => x.PublishDateUtc ?? x.CreatedAtUtc)
         .Take(8)
   .Include(x => x.Media).ThenInclude(m => m.MediaFile)
        .ToListAsync(ct);
     ViewBag.OpenPostId = open;
      return View(latest);
        }

        [HttpPost]
   [ValidateAntiForgeryToken]
  public async Task<IActionResult> SharePhoto(IFormFile? image, string? caption, CancellationToken ct)
        {
            if (image == null || image.Length == 0)
   return BadRequest(new { ok = false, message = "Görsel seçiniz" });

   var (url, contentType, size) = await SaveFileAsync(image, ct);

            var post = new BlogPost
  {
         Title = string.Empty,
        Slug = CreateSlug(Guid.NewGuid().ToString("n")),
   Summary = caption?.Length > 140 ? caption!.Substring(0, 140) + "…" : (caption ?? string.Empty),
     ContentMarkdown = caption ?? string.Empty,
       IsPublished = true,
         PublishDateUtc = DateTime.UtcNow,
          CreatedAtUtc = DateTime.UtcNow
   };
      var media = new MediaFile
    {
         Url = url,
       ContentType = contentType,
         SizeBytes = size,
  OwnerType = MediaOwnerType.BlogPost
   };
     var bpm = new BlogPostMedia { BlogPost = post, MediaFile = media, DisplayOrder = 0 };
    _db.BlogPostMedias.Add(bpm);
      await _db.SaveChangesAsync(ct);

            // Newsletter bildirimi gönder (arka planda, kendi scope'unda)
            var postId = post.Id;
 _ = Task.Run(async () => {
        try
    {
          await _newsletterService.NotifyNewBlogPostAsync(postId);
    _logger.LogInformation("Newsletter bildirimi tamamlandý. BlogPostId={Id}", postId);
        }
   catch (Exception ex)
      {
    _logger.LogError(ex, "Newsletter bildirimi gönderilemedi. BlogPostId={Id} Error={Error}", 
      postId, ex.Message);
      }
       }, CancellationToken.None);

   return Json(new { ok = true, redirect = Url.Action("Index", "Blog", new { area = "" }) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareArticle(string title, string summary, string contentMarkdown, IFormFile? cover, CancellationToken ct)
        {
        if (string.IsNullOrWhiteSpace(title)) return BadRequest(new { ok = false, message = "Baþlýk zorunlu" });

            var post = new BlogPost
     {
       Title = title.Trim(),
             Slug = CreateSlug(title),
    Summary = summary?.Trim() ?? string.Empty,
    ContentMarkdown = contentMarkdown ?? string.Empty,
 IsPublished = true,
    PublishDateUtc = DateTime.UtcNow,
 CreatedAtUtc = DateTime.UtcNow
       };

         if (cover != null && cover.Length > 0)
            {
 var (url, contentType, size) = await SaveFileAsync(cover, ct);
       var media = new MediaFile
   {
     Url = url,
       ContentType = contentType,
   SizeBytes = size,
     OwnerType = MediaOwnerType.BlogPost
     };
    var bpm = new BlogPostMedia { BlogPost = post, MediaFile = media, DisplayOrder = 0 };
       _db.BlogPostMedias.Add(bpm);
       }
    else
        {
        await _db.BlogPosts.AddAsync(post, ct);
  }

            await _db.SaveChangesAsync(ct);

  // Newsletter bildirimi gönder (arka planda, kendi scope'unda)
   var postId = post.Id;
      _ = Task.Run(async () => {
          try
          {
              await _newsletterService.NotifyNewBlogPostAsync(postId);
        _logger.LogInformation("Newsletter bildirimi tamamlandý. BlogPostId={Id}", postId);
    }
  catch (Exception ex)
  {
            _logger.LogError(ex, "Newsletter bildirimi gönderilemedi. BlogPostId={Id} Error={Error}", 
  postId, ex.Message);
      }
    }, CancellationToken.None);

    return Json(new { ok = true, redirect = Url.Action("Index", "Blog", new { area = "" }) });
  }

   private async Task<(string url, string contentType, long size)> SaveFileAsync(IFormFile file, CancellationToken ct)
        {
        var y = DateTime.UtcNow.Year;
     var m = DateTime.UtcNow.Month;
 var rel = Path.Combine("uploads", y.ToString(), m.ToString("D2"));
            var dir = Path.Combine(_env.WebRootPath, rel);
        Directory.CreateDirectory(dir);
            var name = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(dir, name);
            await using (var fs = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fs, ct);
    }
    var url = "/" + Path.Combine(rel, name).Replace("\\", "/");
       return (url, file.ContentType, file.Length);
}

        private static string CreateSlug(string input)
        {
     var s = (input ?? string.Empty).Trim().ToLowerInvariant();
      foreach (var c in new[] { ' ', '/', '\\', '#', '?', '&', ':', ';', '.', ',', '"', '\'', '(', ')', '[', ']', '{', '}', '|' })
  s = s.Replace(c, '-');
            while (s.Contains("--")) s = s.Replace("--", "-");
   if (s.Length > 100) s = s.Substring(0, 100);
    if (string.IsNullOrWhiteSpace(s)) s = Guid.NewGuid().ToString("n");
   return s;
        }
    }
}
