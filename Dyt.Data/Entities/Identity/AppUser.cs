using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // IdentityUser temel sınıfını kullanmak için

namespace Dyt.Data.Entities.Identity // Kimlik yönetimi için ad alanı
{
    /// <summary>
    /// Uygulamadaki kullanıcı varlığı.
    /// Diyetisyen tek kullanıcı senaryosu için yeterli alanları içerir.
    /// </summary>
    public class AppUser : IdentityUser<int> // Birincil anahtar tipi int olan Identity kullanıcısı
    {
        public string? FullName { get; set; } // Ad Soyad bilgisini tutar
        public bool IsActive { get; set; } = true; // Kullanıcının aktiflik durumu

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow; // Kullanıcı kaydının oluşturulma zamanı
        public DateTime? UpdatedAtUtc { get; set; } // Kullanıcı kaydının güncellenme zamanı (opsiyonel)
    }
}
