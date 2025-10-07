using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.Notifications
{
    /// <summary>
    /// Basit e-posta gönderim arayüzü. Gerçek entegrasyon daha sonra baðlanacaktýr.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Belirtilen alýcýya konu ve içerik ile e-posta gönderir.
        /// </summary>
        Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
    }
}
