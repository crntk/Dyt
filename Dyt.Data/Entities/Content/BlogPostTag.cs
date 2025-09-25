using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Entities.Content // İçerik varlıkları ad alanı
{
    /// <summary>
    /// BlogPost ile Tag arasında çoktan çoğa ilişkiyi tutan köprü varlık.
    /// </summary>
    public class BlogPostTag // Junction/bridge tablo
    {
        public int BlogPostId { get; set; } // İlişkili blog yazısının anahtarı
        public BlogPost BlogPost { get; set; } = null! // Blog yazısı navigasyonu
            ; // Navigasyon satırı ayrık tutuldu

        public int TagId { get; set; } // İlişkili etiketin anahtarı
        public Tag Tag { get; set; } = null! // Etiket navigasyonu
            ; // Navigasyon satırı ayrık tutuldu
    }
}

