using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Entities.Content // İçerik varlıkları ad alanı
{
    /// <summary>
    /// Blog gönderisine iliştirilmiş medya dosyalarını ve gösterim sırasını tutar.
    /// </summary>
    public class BlogPostMedia // Blog ile MediaFile arası 1:N yardımcı tablo
    {
        public int Id { get; set; } // Birincil anahtar
        public int BlogPostId { get; set; } // İlişkili blog yazısı anahtarı
        public BlogPost BlogPost { get; set; } = null! // Blog navigasyonu
            ; // Navigasyon ayrı satırda
        public int MediaFileId { get; set; } // İlişkili medya dosyası anahtarı
        public MediaFile MediaFile { get; set; } = null! // Media navigasyonu
            ; // Navigasyon ayrı satırda
        public int DisplayOrder { get; set; } // Medyanın listede gösterim sırası
    }
}

