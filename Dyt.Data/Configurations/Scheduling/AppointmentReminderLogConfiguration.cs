using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Scheduling; // Hatırlatma log varlığı için
using Microsoft.EntityFrameworkCore; // EF çekirdek
using Microsoft.EntityFrameworkCore.Metadata.Builders; // Konfig arayüzü

namespace Dyt.Data.Configurations.Scheduling // Konfigürasyon ad alanı
{
    /// <summary>
    /// AppointmentReminderLog varlığının tablo ayarlarını tanımlar.
    /// </summary>
    public class AppointmentReminderLogConfiguration : IEntityTypeConfiguration<AppointmentReminderLog> // Konfig sınıfı
    {
        /// <summary>
        /// İlişki, uzunluk ve concurrency ayarlarını uygular.
        /// </summary>
        public void Configure(EntityTypeBuilder<AppointmentReminderLog> b) // Uygulama metodu
        {
            b.HasOne(x => x.Appointment).WithMany().HasForeignKey(x => x.AppointmentId); // Randevu ilişkisi
            b.Property(x => x.Channel).IsRequired().HasMaxLength(20); // Kanal adı sınırı
            b.Property(x => x.ProviderMessageId).HasMaxLength(120); // Sağlayıcı mesaj kimliği
            b.Property(x => x.ErrorMessage).HasMaxLength(500); // Hata mesajı sınırı
            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency
        }
    }
}

