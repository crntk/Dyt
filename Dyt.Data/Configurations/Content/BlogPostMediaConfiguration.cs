using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Content; // BlogPostMedia için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfigürasyon arayüzü

namespace Dyt.Data.Configurations.Content // Konfigürasyon ad alanı
{
    /// <summary>
    /// BlogPostMedia varlığının ilişkilerini ve alan kurallarını tanımlar.
    /// </summary>
    public class BlogPostMediaConfiguration : IEntityTypeConfiguration<BlogPostMedia> // Konfig sınıfı
    {
        /// <summary>
        /// İlişkiler ve sıralama sütunu konfigürasyonu uygulanır.
        /// </summary>
        public void Configure(EntityTypeBuilder<BlogPostMedia> b) // Uygulama metodu
        {
            b.HasOne(x => x.BlogPost).WithMany(p => p.Media).HasForeignKey(x => x.BlogPostId); // Blog ilişkisi
            b.HasOne(x => x.MediaFile).WithMany().HasForeignKey(x => x.MediaFileId); // Media ilişkisi
            b.Property(x => x.DisplayOrder).HasDefaultValue(0); // Varsayılan sıralama
        }
    }
}

