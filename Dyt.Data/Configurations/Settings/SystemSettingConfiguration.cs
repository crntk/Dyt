using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Settings; // SystemSetting için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfig arayüzü

namespace Dyt.Data.Configurations.Settings // Konfigürasyon ad alanı
{
    /// <summary>
    /// SystemSetting varlığının tablo ayarlarını tanımlar.
    /// </summary>
    public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting> // Konfig sınıfı
    {
        /// <summary>
        /// Key/Value alan kısıtları ve benzersizlik kurallarını uygular.
        /// </summary>
        public void Configure(EntityTypeBuilder<SystemSetting> b) // Uygulama metodu
        {
            b.Property(x => x.Key).IsRequired().HasMaxLength(80); // Anahtar alanı
            b.Property(x => x.Value).IsRequired(); // Değer alanı
            b.HasIndex(x => x.Key).IsUnique(); // Anahtar benzersiz
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency
        }
    }
}

