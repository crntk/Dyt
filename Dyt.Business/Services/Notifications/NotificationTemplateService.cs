using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Business.Interfaces.Notifications; // Arayüzü uygulamak için
using System.Text; // StringBuilder için

namespace Dyt.Business.Services.Notifications // Bildirim servisleri ad alanı
{
    /// <summary>
    /// SMS şablonlarını basit değişkenlerle oluşturan servis.
    /// İleride veritabanındaki SmsTemplate kayıtlarıyla entegre edilebilir.
    /// </summary>
    public class NotificationTemplateService : INotificationTemplateService // Arayüz uygulaması
    {
        /// <summary>
        /// Randevu hatırlatma SMS metnini üretir.
        /// </summary>
        public string RenderReminder(string clientName, DateOnly date, TimeOnly start, string confirmYesUrl, string confirmNoUrl) // Hatırlatma metni üretimi
        {
            var sb = new StringBuilder(); // Metni birleştirmek için StringBuilder kullanıyorum
            sb.Append($"Merhaba {clientName}, "); // Hitap kısmını ekliyorum
            sb.Append($"{date:yyyy-MM-dd} tarihinde saat {start:HH\\:mm} randevunuz var. "); // Tarih ve saat bilgisini ekliyorum
            sb.Append($"Gelecekseniz: {confirmYesUrl} "); // Evet linkini ekliyorum
            sb.Append($"Gelemeyecekseniz: {confirmNoUrl}"); // Hayır linkini ekliyorum
            return sb.ToString(); // Metni döndürüyorum
        }

        /// <summary>
        /// 2 saat kala hâlâ yanıt yoksa iç bildirim metnini üretir.
        /// </summary>
        public string RenderTwoHourNoResponseAlert(string clientName, DateOnly date, TimeOnly start) // 2 saat kala uyarı metni
        {
            return $"Uyarı: {clientName} adlı danışanın {date:yyyy-MM-dd} {start:HH\\:mm} randevusu için hâlâ onay yanıtı yok."; // Basit uyarı metnini döndürüyorum
        }
    }
}

