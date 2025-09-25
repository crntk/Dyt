using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Entities.Common // Ortak entity tiplerinin tutulduğu ad alanı
{
    /// <summary>
    /// Günlükleme/audit ve soft-delete alanlarını bir arada sağlayan temel sınıf.
    /// Tüm iş varlıkları bu sınıftan türetildiğinde izlenebilirlik standart olur.
    /// </summary>
    public abstract class AuditableEntity : BaseEntity, ISoftDelete // Hem BaseEntity hem de ISoftDelete özelliklerini birleştirir
    {
        public bool IsDeleted { get; set; } // Soft-delete bayrağı
        public DateTime? DeletedAtUtc { get; set; } // Soft-delete zamanı (opsiyonel)
    }
}
