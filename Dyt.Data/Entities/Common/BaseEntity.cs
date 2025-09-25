using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // Timestamp/RowVersion için gerekli

namespace Dyt.Data.Entities.Common // Ortak entity tiplerinin tutulduğu ad alanı
{
    /// <summary>
    /// Tüm varlıkların ortak alanlarını tutan temel sınıf.
    /// Kimlik, oluşturulma/güncellenme tarihi ve eşzamanlılık kontrolü içerir.
    /// </summary>
    public abstract class BaseEntity // Diğer varlıkların kalıtım alacağı temel sınıf
    {
        public int Id { get; set; } // Birincil anahtar alanı

        public DateTime CreatedAtUtc { get; set; } // Kaydın oluşturulduğu UTC tarih
        public DateTime? UpdatedAtUtc { get; set; } // Kaydın son güncellendiği UTC tarih (opsiyonel)

        [Timestamp] // EF Core'un eşzamanlılık kontrolü için kullandığı özel işaret
        public byte[] RowVersion { get; set; } = Array.Empty<byte>(); // Eşzamanlılık için bayt dizisi (veritabanında rowversion/timestamp)
    }
}

