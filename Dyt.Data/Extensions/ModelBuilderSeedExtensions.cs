using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Settings; // Varsayılan ayarlar için
using Microsoft.EntityFrameworkCore; // ModelBuilder için

namespace Dyt.Data.Extensions // Uzantı ad alanı
{
    /// <summary>
    /// Başlangıç verileri için seed uzantıları.
    /// </summary>
    public static class ModelBuilderSeedExtensions // Statik uzantı sınıfı
    {
        /// <summary>
        /// Sistem ayarları ve örnek kayıtların eklenmesine aracılık eder.
        /// Identity kullanıcı ve roller Business/Web tarafında run-time seed ile eklenebilir.
        /// </summary>
        public static void Seed(this ModelBuilder mb) // Uzantı metodu
        {
            mb.Entity<SystemSetting>().HasData( // SystemSetting başlangıç verileri
                new SystemSetting { Id = 1, Key = "ReminderHoursBefore", Value = "24", CreatedAtUtc = DateTime.UtcNow, IsDeleted = false, RowVersion = Array.Empty<byte>() }, // 24 saat kala hatırlatma
                new SystemSetting { Id = 2, Key = "TwoHourAlertBefore", Value = "2", CreatedAtUtc = DateTime.UtcNow, IsDeleted = false, RowVersion = Array.Empty<byte>() }, // 2 saat kala uyarı
                new SystemSetting { Id = 3, Key = "BaseUrl", Value = "https://example.com", CreatedAtUtc = DateTime.UtcNow, IsDeleted = false, RowVersion = Array.Empty<byte>() } // Taban URL örneği
            ); // HasData sonu
        }
    }
}

