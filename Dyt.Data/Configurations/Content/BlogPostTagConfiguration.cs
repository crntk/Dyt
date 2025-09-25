using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Content; // Köprü varlık için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfigürasyon arayüzü

namespace Dyt.Data.Configurations.Content // Konfigürasyon ad alanı
{
    /// <summary>
    /// BlogPostTag köprü tablosunun anahtar ve ilişkilerini tanımlar.
    /// </summary>
    public class BlogPostTagConfiguration : IEntityTypeConfiguration<BlogPostTag> // Konfig sınıfı
    {
        /// <summary>
        /// Birleşik anahtar ve ilişkileri kurar.
        /// </summary>
        public void Configure(EntityTypeBuilder<BlogPostTag> b) // Uygulama metodu
        {
            b.HasKey(x => new { x.BlogPostId, x.TagId }); // Birleşik anahtar
            b.HasOne(x => x.BlogPost).WithMany(p => p.BlogPostTags).HasForeignKey(x => x.BlogPostId); // Blog ilişkisi
            b.HasOne(x => x.Tag).WithMany(t => t.BlogPostTags).HasForeignKey(x => x.TagId); // Tag ilişkisi
        }
    }
}

