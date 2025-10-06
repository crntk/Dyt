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
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            pageSize = Math.Min(pageSize, 20);

            var now = DateTime.UtcNow;
            var query = _db.BlogPosts.AsNoTracking()
                .Where(p => p.IsPublished && (p.PublishDateUtc == null || p.PublishDateUtc <= now))
                .OrderByDescending(p => p.PublishDateUtc ?? p.CreatedAtUtc);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(items);
        }

        [HttpGet("/blog/{slug}")]
        public async Task<IActionResult> Post(string slug, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var post = await _db.BlogPosts.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished && (p.PublishDateUtc == null || p.PublishDateUtc <= now), ct);
            if (post == null) return NotFound();

            var html = Markdown.ToHtml(post.ContentMarkdown ?? string.Empty);
            html = _sanitizer.Sanitize(html);
            ViewBag.ContentHtml = html;
            return View(post);
        }
    }
}
