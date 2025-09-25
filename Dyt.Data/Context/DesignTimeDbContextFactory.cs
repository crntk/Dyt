using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // DbContextOptionsBuilder için
using Microsoft.EntityFrameworkCore.Design; // IDesignTimeDbContextFactory için

namespace Dyt.Data.Context // Tasarım zamanı DbContext fabrikasının bulunduğu ad alanı
{
    /// <summary>
    /// Migration oluşturma gibi tasarım zamanı işlemlerinde DbContext üretir.
    /// CLI komutları bu fabrikayı kullanır.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext> // Tasarım zamanı fabrikası
    {
        /// <summary>
        /// CLI tarafından çağrılır ve geçici bir AppDbContext örneği üretir.
        /// </summary>
        public AppDbContext CreateDbContext(string[] args) // CLI'nin girdi parametreleri alınır
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>(); // DbContext seçeneklerini oluştur
            optionsBuilder.UseSqlServer( // SQL Server sağlayıcısını seç
                "Server=DESKTOP-H8P7QQ0;Database=DytDb;Trusted_Connection=True;TrustServerCertificate=True;"); // Geliştirme için basit bağlantı dizesi

            return new AppDbContext(optionsBuilder.Options); // Seçeneklerle DbContext oluştur ve döndür
        }
    }
}

