using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Entities.Content // İçerik varlıkları ad alanı
{
    /// <summary>
    /// Blog yazıları için etiket varlığı. Listeleme ve filtreleme amacıyla kullanılır.
    /// </summary>
    public class Tag // Basit bir sözlük tablosu
    {
        public int Id { get; set; } // Birincil anahtar
        public string Name { get; set; } = string.Empty; // Etiket görünen adı
        public string Slug { get; set; } = string.Empty; // URL dostu kısa ad
        public bool IsDeleted { get; set; } // Soft-delete işareti
        public DateTime CreatedAtUtc { get; set; } // Oluşturulma zamanı
        public DateTime? UpdatedAtUtc { get; set; } // Güncellenme zamanı (opsiyonel)
        public byte[] RowVersion { get; set; } = Array.Empty<byte>(); // Concurrency alanı
        public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>(); // İlişkili blog-post köprüleri
    }
}

