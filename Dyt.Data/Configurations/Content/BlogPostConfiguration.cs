using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Content; // BlogPost için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfigürasyon arayüzü

namespace Dyt.Data.Configurations.Content // Konfigürasyon ad alanı
{
    /// <summary>
    /// BlogPost varlığına ait tablo konfigürasyonlarını içerir.
    /// </summary>
    public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost> // Konfig sınıfı
    {
        /// <summary>
        /// Özellik kısıtlarını, indeksleri ve concurrency ayarını uygular.
        /// </summary>
        public void Configure(EntityTypeBuilder<BlogPost> b) // Uygulama metodu
        {
            b.Property(x => x.Title).IsRequired().HasMaxLength(160); // Başlık zorunlu ve 160 karakter
            b.Property(x => x.Slug).IsRequired().HasMaxLength(180); // Slug zorunlu ve 180 karakter
            b.HasIndex(x => x.Slug).IsUnique(); // Slug benzersiz olsun
            b.Property(x => x.Summary).HasMaxLength(500); // Özet metin sınırı
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency kontrolü
        }
    }
}

