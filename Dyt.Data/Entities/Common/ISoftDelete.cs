using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Entities.Common // Ortak sözleşmelerin tutulduğu ad alanı
{
    /// <summary>
    /// Soft-delete mantığı uygulayan varlıkların uyması gereken sözleşme.
    /// Silme işlemi fiziksel değil, mantıksal olarak işaretlenir.
    /// </summary>
    public interface ISoftDelete // Soft-delete işaretlemesi için arayüz
    {
        bool IsDeleted { get; set; } // Kayıt silinmiş gibi davranılsın mı
        DateTime? DeletedAtUtc { get; set; } // Silinme zamanı (opsiyonel, raporlama için faydalı)
    }
}
