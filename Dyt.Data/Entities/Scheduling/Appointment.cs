using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için
using Dyt.Data.Enums; // Randevu durum enumları için

namespace Dyt.Data.Entities.Scheduling // Randevu ve zamanlama varlıkları için ad alanı
{
    /// <summary>
    /// Danışan randevularını temsil eden varlık.
    /// Tarih/saat, danışan bilgileri ve onay akışıyla ilgili alanları içerir.
    /// </summary>
    public class Appointment : AuditableEntity // Audit ve soft-delete alanlarını hazır almak için
    {
        public DateOnly AppointmentDate { get; set; } // Randevunun günü
        public TimeOnly StartTime { get; set; } // Randevunun başlangıç saati
        public TimeOnly EndTime { get; set; } // Randevunun bitiş saati

        public string ClientName { get; set; } = string.Empty; // Danışanın adı soyadı
        public string ClientPhone { get; set; } = string.Empty; // Danışanın telefon numarası (SMS için zorunlu)
        public string? ClientEmail { get; set; } // Danışanın e-posta adresi (opsiyonel)

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled; // Randevunun genel durumu (ör. Scheduled)
        public ConfirmationState ConfirmationState { get; set; } = ConfirmationState.Yanıtlanmadı; // Danışanın yanıt durumu

        public string ConfirmationTokenYes { get; set; } = string.Empty; // Gelecek işaretleme linki için token
        public string ConfirmationTokenNo { get; set; } = string.Empty; // Gelmeyecek işaretleme linki için token
        public DateTime? ConfirmationAtUtc { get; set; } // Danışanın yanıt verdiği UTC zaman

        public bool ReminderSent { get; set; } // Hatırlatma SMS'i gönderildi mi
        public bool TwoHourNoResponseAlertShown { get; set; } // 2 saat kala yanıt yok uyarısı gösterildi mi

        public string? Notes { get; set; } // Diyetisyenin randevuya özel notu (opsiyonel)
    }
}
