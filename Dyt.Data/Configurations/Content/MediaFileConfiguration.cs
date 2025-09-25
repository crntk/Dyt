using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Content; // MediaFile için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfigürasyon arayüzü

namespace Dyt.Data.Configurations.Content // Konfigürasyon ad alanı
{
    /// <summary>
    /// MediaFile varlığına ait sütun ve uzunluk kurallarını tanımlar.
    /// </summary>
    public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile> // Konfig sınıfı
    {
        /// <summary>
        /// Uzunluklar, zorunluluklar ve concurrency alanını ayarlar.
        /// </summary>
        public void Configure(EntityTypeBuilder<MediaFile> b) // Uygulama metodu
        {
            b.Property(x => x.Url).IsRequired().HasMaxLength(500); // URL zorunlu ve 500 karakter
            b.Property(x => x.Title).HasMaxLength(160); // Başlık sınırı
            b.Property(x => x.AltText).HasMaxLength(160); // Alternatif metin sınırı
            b.Property(x => x.ContentType).IsRequired().HasMaxLength(80); // MIME türü sınırı
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency
        }
    }
}

