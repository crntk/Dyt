using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için

namespace Dyt.Data.Entities.Content // İçerik varlıkları ad alanı
{
    /// <summary>
    /// Ana sayfadaki diyetisyen profil/kurumsal içeriklerini tutar.
    /// </summary>
    public class DietitianProfile : AuditableEntity // Audit ve soft-delete sahaları için kalıtım
    {
        public string HeaderTitle { get; set; } = string.Empty; // Hero bölümündeki başlık

        public string AboutMarkdown { get; set; } = string.Empty; // Hakkında metni markdown olarak

        public string? ProfilePhotoUrl { get; set; } // Profil fotoğrafı dosya yolu

        public string? HeroBackgroundUrl { get; set; } // Arka plan görseli dosya yolu

        public string? ContactPhone { get; set; } // İletişim telefonu

        public string? ContactEmail { get; set; } // İletişim e-postası

        public string? Address { get; set; } // Açık adres bilgisi
    }
}

