using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity; // IdentityRole temel sınıfı için

namespace Dyt.Data.Entities.Identity // Kimlik ve rol yönetimi için ad alanı
{
    /// <summary>
    /// Uygulamadaki rol varlığı.
    /// İleride asistan vb. roller eklemek gerekirse hazır dursun.
    /// </summary>
    public class AppRole : IdentityRole<int> // Birincil anahtar tipi int olan Identity rolü
    {
        public string? Description { get; set; } // Rolün kısa açıklaması (opsiyonel)
    }
}
