using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Settings; // SmsTemplate için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfig arayüzü

namespace Dyt.Data.Configurations.Settings // Konfigürasyon ad alanı
{
    /// <summary>
    /// SmsTemplate varlığına ait tablo ayarlarını tanımlar.
    /// </summary>
    public class SmsTemplateConfiguration : IEntityTypeConfiguration<SmsTemplate> // Konfig sınıfı
    {
        /// <summary>
        /// Anahtar ve içerik sütun kısıtları tanımlanır.
        /// </summary>
        public void Configure(EntityTypeBuilder<SmsTemplate> b) // Uygulama metodu
        {
            b.Property(x => x.TemplateKey).IsRequired().HasMaxLength(80); // Şablon anahtarı
            b.Property(x => x.Content).IsRequired(); // Şablon metni
            b.HasIndex(x => x.TemplateKey).IsUnique(); // Anahtar benzersiz
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency
        }
    }
}

