using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Content; // Tag varlığı için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfigürasyon arayüzü

namespace Dyt.Data.Configurations.Content // Konfigürasyon ad alanı
{
    /// <summary>
    /// Tag varlığının tablo ayarlarını tanımlar.
    /// </summary>
    public class TagConfiguration : IEntityTypeConfiguration<Tag> // Konfig sınıfı
    {
        /// <summary>
        /// Alan kısıtlarını ve indeksleri uygular.
        /// </summary>
        public void Configure(EntityTypeBuilder<Tag> b) // Uygulama metodu
        {
            b.Property(x => x.Name).IsRequired().HasMaxLength(80); // Etiket adı zorunlu ve 80 karakter
            b.Property(x => x.Slug).IsRequired().HasMaxLength(100); // Slug zorunlu ve 100 karakter
            b.HasIndex(x => x.Slug).IsUnique(); // Slug benzersiz
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency
        }
    }
}

