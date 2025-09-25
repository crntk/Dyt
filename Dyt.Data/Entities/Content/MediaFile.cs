using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için
using Dyt.Data.Enums; // MediaOwnerType için

namespace Dyt.Data.Entities.Content // İçerik varlıkları ad alanı
{
    /// <summary>
    /// Yüklenen medya dosyalarının meta bilgilerini tutar.
    /// Görsel yolu, içerik türü ve sahibine ilişkin bilgiler içerir.
    /// </summary>
    public class MediaFile : AuditableEntity // Audit + soft delete desteği
    {
        public string Url { get; set; } = string.Empty; // Dosyanın erişim URL'si
        public string? Title { get; set; } // Dosya başlığı (opsiyonel)
        public string? AltText { get; set; } // Alternatif metin (erişilebilirlik için)
        public string ContentType { get; set; } = "image/jpeg"; // MIME türü
        public long SizeBytes { get; set; } // Bayt cinsinden boyut
        public MediaOwnerType OwnerType { get; set; } // Medyanın tipik sahibi (profil, blogpost vb.)
        public int? OwnerId { get; set; } // Sahip varlığın kimliği (opsiyonel; mantıksal eşleme)
    }
}

