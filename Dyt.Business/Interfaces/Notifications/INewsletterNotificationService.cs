using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.Notifications
{
    /// <summary>
  /// Newsletter bildirimi gönderme servisi
    /// </summary>
    public interface INewsletterNotificationService
    {
        /// <summary>
     /// Yeni blog yazýsý yayýnlandýðýnda tüm abonelere bildirim gönderir
     /// </summary>
        Task NotifyNewBlogPostAsync(int blogPostId, CancellationToken ct = default);
    }
}
