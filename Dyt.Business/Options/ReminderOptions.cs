using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Options // Seçenek sınıfları için ad alanını tanımlıyorum
{
    /// <summary>
    /// Randevu hatırlatma ve uyarı zamanlamalarına ait konfigürasyon seçenekleri.
    /// </summary>
    public class ReminderOptions // Hatırlatma seçeneklerini temsil eden sınıf
    {
        public const string SectionName = "Reminder"; // appsettings içindeki bölüm adını sabitliyorum

        public int HoursBefore { get; set; } = 24; // Randevudan kaç saat önce hatırlatma yapılacağını tutuyorum
        public int NoResponseAlertHoursBefore { get; set; } = 2; // Yanıt yok uyarısı için kalan saat eşiğini tutuyorum
        public int ScanIntervalSeconds { get; set; } = 300; // Arka plan servisinin tarama aralığını saniye cinsinden tutuyorum
    }
}

