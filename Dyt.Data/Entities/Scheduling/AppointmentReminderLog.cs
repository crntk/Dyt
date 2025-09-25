using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity kullanımı için
using Dyt.Data.Enums; // ReminderType enum'u için

namespace Dyt.Data.Entities.Scheduling // Randevu ve zamanlama varlıklarının ad alanı
{
    /// <summary>
    /// Gönderilen hatırlatma/uyarı mesajlarının kayıtlarını tutar. Geriye dönük izlenebilirlik sağlar.
    /// </summary>
    public class AppointmentReminderLog : AuditableEntity // Audit ve soft-delete alanlarını kalıtmak için
    {
        public int AppointmentId { get; set; } // İlişkili randevunun anahtar değeri

        public Appointment Appointment { get; set; } = null!; // Randevu navigasyon özelliği

        public ReminderType Type { get; set; } // Hatırlatma/uyarı türü (ör. BeforeXHours)

        public string Channel { get; set; } = "SMS"; // Hangi kanal kullanıldı (SMS / Email)

        public bool Success { get; set; } // Gönderim başarılı mıydı

        public string? ProviderMessageId { get; set; } // SMS sağlayıcının döndürdüğü mesaj kimliği

        public string? ErrorMessage { get; set; } // Varsa hata detayı

        public DateTime SentAtUtc { get; set; } // Gönderim zamanı (UTC)
    }
}

