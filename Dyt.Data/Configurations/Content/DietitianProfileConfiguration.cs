using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Content; // DietitianProfile için
using Microsoft.EntityFrameworkCore; // EF çekirdek türleri için
using Microsoft.EntityFrameworkCore.Metadata.Builders; // IEntityTypeConfiguration için

namespace Dyt.Data.Configurations.Content // Konfigürasyon ad alanı
{
    /// <summary>
    /// DietitianProfile varlığının tablo ayarlarını tanımlar.
    /// </summary>
    public class DietitianProfileConfiguration : IEntityTypeConfiguration<DietitianProfile> // Konfig sınıfı
    {
        /// <summary>
        /// EF Core model kurulumunda çağrılır ve sütun kurallarını uygular.
        /// </summary>
        public void Configure(EntityTypeBuilder<DietitianProfile> b) // Konfigürasyon metodu
        {
            b.Property(x => x.HeaderTitle).HasMaxLength(160).IsRequired(); // Başlık zorunlu ve 160 karakter sınırı
            b.Property(x => x.AboutMarkdown).IsRequired(); // Hakkında içeriği zorunlu
            b.Property(x => x.ProfilePhotoUrl).HasMaxLength(500); // Fotoğraf URL uzunluk sınırı
            b.Property(x => x.HeroBackgroundUrl).HasMaxLength(500); // Arka plan URL uzunluk sınırı
            b.Property(x => x.ContactPhone).HasMaxLength(24); // Telefon formatı için makul sınır
            b.Property(x => x.ContactEmail).HasMaxLength(120); // E-posta sınırı
            b.Property(x => x.Address).HasMaxLength(500); // Adres metni sınırı
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency ayarı
        }
    }
}

