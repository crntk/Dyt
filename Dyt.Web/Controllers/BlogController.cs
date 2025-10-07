using System;
using System.Linq;
using System.Threading.Tasks;
using Dyt.Business.Security.Sanitization;
using Dyt.Data.Context;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dyt.Web.Controllers
{
    /// <summary>
    /// Kamuya açýk blog listeleme ve yazý detaylarý.
    /// </summary>
    public class BlogController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IContentSanitizer _sanitizer;

        public BlogController(AppDbContext db, IContentSanitizer sanitizer)
        {
            _db = db;
            _sanitizer = sanitizer;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, int apage = 1, int apageSize = 10, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            pageSize = Math.Min(pageSize, 20);

            if (apage < 1) apage = 1;
            if (apageSize < 1) apageSize = 10;
            apageSize = Math.Min(apageSize, 50);

            var now = DateTime.UtcNow;
            var baseQuery = _db.BlogPosts.AsNoTracking()
                .Where(p => p.IsPublished && (p.PublishDateUtc == null || p.PublishDateUtc <= now))
                .Include(p => p.Media)
                    .ThenInclude(m => m.MediaFile);

            // Sol grid (fotoðraf gönderileri): baþlýðý olmayanlar
            var photosQuery = baseQuery
                .Where(p => string.IsNullOrEmpty(p.Title))
                .OrderByDescending(p => p.PublishDateUtc ?? p.CreatedAtUtc);

            var totalPhotos = await photosQuery.CountAsync(ct);
            var photos = await photosQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            // Sað sütun (yazýlar ve makaleler): baþlýðý olanlar
            var articlesQuery = baseQuery
                .Where(p => !string.IsNullOrEmpty(p.Title))
                .OrderByDescending(p => p.PublishDateUtc ?? p.CreatedAtUtc);

            var totalArticles = await articlesQuery.CountAsync(ct);
            var articles = await articlesQuery.Skip((apage - 1) * apageSize).Take(apageSize).ToListAsync(ct);

            ViewBag.Articles = articles;
            ViewBag.ArticleTotal = totalArticles;
            ViewBag.ArticlePage = apage;
            ViewBag.ArticlePageSize = apageSize;

            ViewBag.Total = totalPhotos;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(photos);
        }

        [HttpGet("/blog/{slug}")]
        public async Task<IActionResult> Post(string slug, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var post = await _db.BlogPosts.AsNoTracking()
                .Include(p => p.Media)
                    .ThenInclude(m => m.MediaFile)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished && (p.PublishDateUtc == null || p.PublishDateUtc <= now), ct);
            if (post == null) return NotFound();

            var html = Markdown.ToHtml(post.ContentMarkdown ?? string.Empty);
            html = _sanitizer.Sanitize(html);
            ViewBag.ContentHtml = html;
            return View(post);
        }
    }
}
