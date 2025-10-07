using System.Threading;
using System.Threading.Tasks;
using Dyt.Business.Interfaces.Notifications;
using Microsoft.Extensions.Logging;

namespace Dyt.Business.Services.Notifications
{
    /// <summary>
    /// Geliþtirme için basit e-posta gönderici þablonu. Gerçek gönderim sonradan eklenecek.
    /// </summary>
    public class EmailSenderStub : IEmailSender
    {
        private readonly ILogger<EmailSenderStub> _log;
        public EmailSenderStub(ILogger<EmailSenderStub> log)
        {
            _log = log;
        }

        public Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            _log.LogInformation("[EmailStub] To={To} Subject={Subject} BodyLength={Len}", toEmail, subject, htmlBody?.Length ?? 0);
            return Task.CompletedTask;
        }
    }
}
