using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity sınıfını kullanmak için
using System.Collections.Generic; // Koleksiyon tipleri için

namespace Dyt.Data.Entities.Content // İçerik tarafındaki varlıkların ad alanı
{
    /// <summary>
    /// Blog yazısı varlığı. Başlık, slug, özet, içerik ve ilişkilendirilmiş etiket/medya koleksiyonlarını içerir.
    /// </summary>
    public class BlogPost : AuditableEntity // Ortak audit ve soft-delete alanlarını kalıtmak için
    {
        public string Title { get; set; } = string.Empty; // Yazının görünen başlığı

        public string Slug { get; set; } = string.Empty; // URL dostu kısa ad (benzersiz olacak)

        public string Summary { get; set; } = string.Empty; // Listeleme sayfaları için kısa özet

        public string ContentMarkdown { get; set; } = string.Empty; // İçerik markdown formatında tutulur

        public bool IsPublished { get; set; } // İçerik yayında mı

        public DateTime? PublishDateUtc { get; set; } // Yayın tarihi (yayında değilse boş kalabilir)

        public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>(); // N:N tag köprü kayıtları

        public ICollection<BlogPostMedia> Media { get; set; } = new List<BlogPostMedia>(); // Yazıya bağlı medya öğeleri
    }
}

