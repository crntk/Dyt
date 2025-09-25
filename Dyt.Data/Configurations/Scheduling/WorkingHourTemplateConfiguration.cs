using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Scheduling; // Çalışma şablonu için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfig arayüzü

namespace Dyt.Data.Configurations.Scheduling // Konfigürasyon ad alanı
{
    /// <summary>
    /// Haftalık çalışma saatleri şablonunun tablo ayarlarını tanımlar.
    /// </summary>
    public class WorkingHourTemplateConfiguration : IEntityTypeConfiguration<WorkingHourTemplate> // Konfig sınıfı
    {
        /// <summary>
        /// Zorunluluklar ve concurrency ayarları uygulanır.
        /// </summary>
        public void Configure(EntityTypeBuilder<WorkingHourTemplate> b) // Uygulama metodu
        {
            b.Property(x => x.DayOfWeek).IsRequired(); // Gün alanı zorunlu
            b.Property(x => x.StartTime).IsRequired(); // Başlangıç saati zorunlu
            b.Property(x => x.EndTime).IsRequired(); // Bitiş saati zorunlu
            b.Property(x => x.SlotMinutes).HasDefaultValue(30); // Varsayılan slot süresi
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency
        }
    }
}

