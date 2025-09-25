using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Scheduling; // İstisna varlığı için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfig arayüzü

namespace Dyt.Data.Configurations.Scheduling // Konfigürasyon ad alanı
{
    /// <summary>
    /// Çalışma saatleri istisnalarının tablo ayarlarını tanımlar.
    /// </summary>
    public class WorkingHourExceptionConfiguration : IEntityTypeConfiguration<WorkingHourException> // Konfig sınıfı
    {
        /// <summary>
        /// Sütun zorunlulukları ve concurrency ayarları yapılır.
        /// </summary>
        public void Configure(EntityTypeBuilder<WorkingHourException> b) // Uygulama metodu
        {
            b.Property(x => x.Date).IsRequired(); // Tarih zorunlu
            b.Property(x => x.Type).IsRequired(); // Tür zorunlu
            b.Property(x => x.Note).HasMaxLength(200); // Kısa not alanı
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency
        }
    }
}

