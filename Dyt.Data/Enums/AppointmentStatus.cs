using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Enums // Enum'ların ortak ad alanı
{
    /// <summary>
    /// Randevunun yaşam döngüsündeki genel durumunu ifade eder.
    /// </summary>
    public enum AppointmentStatus // Randevu durumları
    {
        Scheduled = 0, // Planlandı, henüz gerçekleşmedi
        Confirmed = 1, // Danışan gelecek dedi (onaylandı)
        Declined = 2, // Danışan gelmeyecek dedi (reddetti)
        NoShow = 3, // Danışan gelmedi
        Completed = 4, // Randevu yapıldı ve tamamlandı
        Canceled = 5  // Diyetisyen veya sistem tarafından iptal edildi
    }
}

