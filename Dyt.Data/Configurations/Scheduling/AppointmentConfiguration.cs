using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Scheduling; // Appointment varlığı için
using Microsoft.EntityFrameworkCore; // EF Core temel türleri için
using Microsoft.EntityFrameworkCore.Metadata.Builders; // IEntityTypeConfiguration için

namespace Dyt.Data.Configurations.Scheduling // Appointment konfigürasyonu için ad alanı
{
    /// <summary>
    /// Appointment varlığının veritabanı konfigürasyonlarını tanımlar.
    /// Uzunluk kısıtları, indeksler ve concurrency ayarları burada yapılır.
    /// </summary>
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment> // Tür bazlı konfigürasyon sınıfı
    {
        /// <summary>
        /// EF Core tarafından model oluşturulurken çağrılır.
        /// Bu metotta property kuralları ve indeksler tanımlanır.
        /// </summary>
        public void Configure(EntityTypeBuilder<Appointment> b) // Konfigürasyonu uygulayan metot
        {
            b.Property(x => x.ClientName).IsRequired().HasMaxLength(80); // İsim zorunlu ve 80 karakteri geçmesin
            b.Property(x => x.ClientPhone).IsRequired().HasMaxLength(24); // Telefon zorunlu ve 24 karakteri geçmesin
            b.Property(x => x.ClientEmail).HasMaxLength(120); // E-posta opsiyonel ama 120 karakter sınırı olsun
            b.Property(x => x.Notes).HasMaxLength(500); // Not alanı 500 karakteri geçmesin

            b.Property(x => x.ConfirmationTokenYes).HasMaxLength(200); // Gelecek token uzunluk limiti
            b.Property(x => x.ConfirmationTokenNo).HasMaxLength(200); // Gelmeyecek token uzunluk limiti

            b.HasIndex(x => new { x.AppointmentDate, x.StartTime }).IsUnique(); // Aynı gün ve başlangıç saatine ikinci randevu yazılmasın

            b.Property(x => x.RowVersion).IsRowVersion(); // Concurrency için RowVersion işaretlemesi
        }
    }
}
