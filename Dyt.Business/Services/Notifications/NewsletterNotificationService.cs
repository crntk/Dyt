using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dyt.Business.Interfaces.Notifications;
using Dyt.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dyt.Business.Services.Notifications
{
    /// <summary>
    /// Newsletter bildirimi gönderme servisi implementasyonu
    /// </summary>
    public class NewsletterNotificationService : INewsletterNotificationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<NewsletterNotificationService> _logger;

        public NewsletterNotificationService(
            IServiceScopeFactory scopeFactory,
            ILogger<NewsletterNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Yeni blog yazýsý yayýnlandýðýnda tüm aktif abonelere bildirim gönderir
        /// </summary>
        public async Task NotifyNewBlogPostAsync(int blogPostId, CancellationToken ct = default)
        {
            // Yeni bir scope oluþtur - kendi DbContext'i olacak
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            try
            {
                // Blog yazýsýný getir
                var blogPost = await db.BlogPosts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == blogPostId && !b.IsDeleted, ct);

                if (blogPost == null)
                {
                    _logger.LogWarning("Blog post {BlogPostId} bulunamadý", blogPostId);
                    return;
                }

                // Aktif aboneleri getir
                var subscribers = await db.NewsletterSubscribers
                    .AsNoTracking()
                    .Where(n => n.IsActive && n.IsVerified && !n.IsDeleted)
                    .ToListAsync(ct);

                if (!subscribers.Any())
                {
                    _logger.LogInformation("Aktif abone bulunamadý");
                    return;
                }

                _logger.LogInformation("Blog '{Title}' için {Count} aboneye bildirim gönderiliyor",
                    blogPost.Title, subscribers.Count);

                int successCount = 0;
                int failCount = 0;

                // Her aboneye email gönder
                foreach (var subscriber in subscribers)
                {
                    try
                    {
                        var subject = $"?? Yeni Blog Yazýsý: {blogPost.Title}";
                        var body = BuildEmailBody(blogPost, subscriber);

                        await emailSender.SendAsync(subscriber.Email, subject, body, ct);

                        // Son gönderim zamanýný güncelle
                        var dbSubscriber = await db.NewsletterSubscribers
                            .FirstOrDefaultAsync(n => n.Id == subscriber.Id, ct);

                        if (dbSubscriber != null)
                        {
                            dbSubscriber.LastNotificationSentAt = DateTime.UtcNow;
                        }

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Abone {Email} için email gönderilemedi: {Error}", 
     subscriber.Email, ex.Message);
                        failCount++;
                    }
                }

                await db.SaveChangesAsync(ct);

                _logger.LogInformation(
         "Newsletter bildirimleri tamamlandý. Baþarýlý: {Success}, Baþarýsýz: {Failed}", 
       successCount, failCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Newsletter bildirimi gönderilirken kritik hata oluþtu: {Error}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Email içeriðini oluþturur
        /// </summary>
        private string BuildEmailBody(Data.Entities.Content.BlogPost blogPost, Data.Entities.Content.NewsletterSubscriber subscriber)
        {
            var unsubscribeUrl = $"https://yourdomain.com/newsletter/unsubscribe?token={subscriber.UnsubscribeToken}";
            var blogUrl = $"https://yourdomain.com/blog/{blogPost.Slug}";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #4CAF50, #81C784); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .button {{ display: inline-block; padding: 12px 24px; background: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #999; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>?? Yeni Blog Yazýsý!</h1>
    </div>
    <div class='content'>
        <h2>{blogPost.Title}</h2>
        <p>{blogPost.Summary}</p>
        <a href='{blogUrl}' class='button'>Yazýyý Oku</a>
        <p>Sevgilerle,<br>Diyetisyen Riyaza Tair</p>
    </div>
    <div class='footer'>
        <p>Bu e-postayý blog bültenimize abone olduðunuz için alýyorsunuz.</p>
        <p><a href='{unsubscribeUrl}'>Abonelikten çýk</a></p>
    </div>
</body>
</html>";
        }
    }
}
