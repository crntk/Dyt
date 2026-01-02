using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Dyt.Business.Interfaces.Notifications;
using Dyt.Business.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dyt.Business.Services.Notifications
{
    /// <summary>
    /// SMTP ile gerçek email gönderimi yapan servis
    /// </summary>
    public class EmailSenderSmtp : IEmailSender
    {
        private readonly ILogger<EmailSenderSmtp> _log;
        private readonly EmailOptions _options;

        public EmailSenderSmtp(ILogger<EmailSenderSmtp> log, IOptions<EmailOptions> options)
        {
     _log = log;
_options = options.Value;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
        {
            // Eðer gerçek gönderim kapalýysa sadece log
            if (!_options.EnableRealSend)
   {
             _log.LogInformation("[Email] SIMULATED - To={To} Subject={Subject} BodyLength={Len}", 
         toEmail, subject, htmlBody?.Length ?? 0);
    return;
       }

      try
            {
     // SMTP istemcisi oluþtur
       using var smtpClient = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
                {
     Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword),
            EnableSsl = _options.EnableSsl,
               Timeout = 30000 // 30 saniye timeout
     };

            // Email mesajýný oluþtur
   using var mailMessage = new MailMessage
         {
           From = new MailAddress(_options.FromEmail, _options.FromName),
        Subject = subject,
     Body = htmlBody,
      IsBodyHtml = true
     };

         mailMessage.To.Add(toEmail);

       // Email gönder
    await smtpClient.SendMailAsync(mailMessage, ct);

      _log.LogInformation("[Email] SENT - To={To} Subject={Subject}", toEmail, subject);
          }
            catch (Exception ex)
   {
              _log.LogError(ex, "[Email] FAILED - To={To} Subject={Subject} Error={Error}", 
          toEmail, subject, ex.Message);
         throw; // Hatayý yukarý fýrlat ki çaðýran kod farkýnda olsun
            }
        }
    }
}
