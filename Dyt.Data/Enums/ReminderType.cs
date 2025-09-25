using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Enums // Enum'ların ortak ad alanı
{
    /// <summary>
    /// Hatırlatma/uyarı mesajının türünü belirtir.
    /// </summary>
    public enum ReminderType // Hatırlatma çeşitleri
    {
        Initial = 0, // İlk onay/karşılama mesajı (opsiyonel)
        BeforeXHours = 1, // Randevuya X saat kala hatırlatma
        TwoHourNoResponse = 2 // Randevuya 2 saat kala hâlâ yanıt yok uyarısı
    }
}

