using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Utils // Yardımcı tiplerin bulunduğu ad alanını tanımlıyorum
{
    /// <summary>
    /// Gerçek sistem saatini kullanan zaman sağlayıcısı.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider // Arayüzü uygulayan somut sınıf
    {
        public DateTime UtcNow => DateTime.UtcNow; // Anlık UTC zamanı sistemden döndürüyorum
        public DateOnly TodayUtc => DateOnly.FromDateTime(DateTime.UtcNow); // Bugünün UTC tarihini hesaplayıp döndürüyorum
    }
}

