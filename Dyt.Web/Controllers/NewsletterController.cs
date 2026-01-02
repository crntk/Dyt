using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dyt.Data.Context;
using Dyt.Data.Entities.Content;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dyt.Web.Controllers
{
    /// <summary>
    /// Blog bülteni abonelik iþlemleri
  /// </summary>
    public class NewsletterController : Controller
    {
        private readonly AppDbContext _db;
   
        // Ýzin verilen email saðlayýcýlarý
        private static readonly string[] AllowedDomains = new[]
  {
       "@gmail.com",
            "@hotmail.com",
          "@outlook.com",
            "@yahoo.com",
     "@yandex.com"
    };

        public NewsletterController(AppDbContext db)
    {
          _db = db;
      }

    /// <summary>
        /// Bültene abone ol
        /// POST /newsletter/subscribe
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
     public async Task<IActionResult> Subscribe(string email, CancellationToken ct)
        {
            // Email validasyonu
      if (string.IsNullOrWhiteSpace(email))
            {
    return Json(new { ok = false, message = "Lütfen e-posta adresinizi girin." });
    }

      email = email.Trim().ToLowerInvariant();

// Email format kontrolü
            if (!IsValidEmail(email))
   {
         return Json(new { ok = false, message = "Geçersiz e-posta formatý." });
     }

            // Ýzin verilen domain kontrolü
            if (!AllowedDomains.Any(domain => email.EndsWith(domain, StringComparison.OrdinalIgnoreCase)))
    {
        var allowedList = string.Join(", ", AllowedDomains);
         return Json(new { ok = false, message = $"Lütfen geçerli bir e-posta adresi kullanýn ({allowedList})." });
            }

            // Zaten abone mi kontrol et
 var existing = await _db.NewsletterSubscribers
                .FirstOrDefaultAsync(n => n.Email == email && !n.IsDeleted, ct);

            if (existing != null)
            {
           if (existing.IsActive)
    {
           return Json(new { ok = false, message = "Bu e-posta adresi zaten kayýtlý." });
        }
    else
       {
          // Pasif aboneliði tekrar aktif et
         existing.IsActive = true;
    existing.UpdatedAtUtc = DateTime.UtcNow;
     await _db.SaveChangesAsync(ct);
  return Json(new { ok = true, message = "? Aboneliðiniz yeniden aktif edildi! Yeni blog yazýlarýndan haberdar olacaksýnýz." });
    }
   }

// Yeni abone oluþtur
            var subscriber = new NewsletterSubscriber
{
       Email = email,
                IsActive = true,
       IsVerified = true,
                UnsubscribeToken = Guid.NewGuid().ToString(),
     CreatedAtUtc = DateTime.UtcNow,
       UpdatedAtUtc = DateTime.UtcNow
        };

  _db.NewsletterSubscribers.Add(subscriber);
     await _db.SaveChangesAsync(ct);

     return Json(new { ok = true, message = "? Baþarýyla abone oldunuz! Yeni blog yazýlarýndan haberdar olacaksýnýz." });
        }

        /// <summary>
        /// Abonelikten çýk
   /// GET /newsletter/unsubscribe?token=xxx
        /// </summary>
        [HttpGet("/newsletter/unsubscribe")]
        public async Task<IActionResult> Unsubscribe(string token, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(token))
     {
        return View("UnsubscribeResult", model: "Geçersiz baðlantý.");
 }

            var subscriber = await _db.NewsletterSubscribers
    .FirstOrDefaultAsync(n => n.UnsubscribeToken == token && !n.IsDeleted, ct);

         if (subscriber == null)
            {
       return View("UnsubscribeResult", model: "Abonelik bulunamadý.");
        }

  if (!subscriber.IsActive)
   {
           return View("UnsubscribeResult", model: "Aboneliðiniz zaten iptal edilmiþ.");
            }

  subscriber.IsActive = false;
            subscriber.UpdatedAtUtc = DateTime.UtcNow;
 await _db.SaveChangesAsync(ct);

  return View("UnsubscribeResult", model: "? Aboneliðiniz baþarýyla iptal edildi. Artýk blog bildirimleri almayacaksýnýz.");
  }

        /// <summary>
      /// Email format validasyonu
   /// </summary>
        private static bool IsValidEmail(string email)
        {
       if (string.IsNullOrWhiteSpace(email))
     return false;

            try
  {
            // Basit regex kontrolü
var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
         return regex.IsMatch(email);
       }
  catch
         {
     return false;
        }
        }
    }
}
