using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Utils // Yardımcı tiplerin bulunduğu ad alanını tanımlıyorum
{
    /// <summary>
    /// Test edilebilir ve kontrol edilebilir zaman kaynağı için soyutlama.
    /// </summary>
    public interface IDateTimeProvider // Zaman sağlayıcısı arayüzü
    {
        DateTime UtcNow { get; } // Şu anki UTC zamanı yalnızca okunur özellik olarak bildiriyorum
        DateOnly TodayUtc { get; } // Bugünün UTC tarihini yalnızca okunur özellik olarak bildiriyorum
    }
}
