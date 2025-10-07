using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Content; // İçerik varlıkları için
using Dyt.Data.Entities.Identity; // Identity varlıkları için
using Dyt.Data.Entities.Scheduling; // Randevu varlıkları için
using Dyt.Data.Entities.Settings; // Ayar varlıkları için
using Dyt.Data.Extensions; // Soft-delete uzantısı için
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // IdentityDbContext için
using Microsoft.EntityFrameworkCore; // DbContext ve ModelBuilder için

namespace Dyt.Data.Context // Veritabanı bağlamının bulunduğu ad alanı
{
    /// <summary>
    /// Uygulamanın EF Core veritabanı bağlamı.
    /// Identity tabloları dahil tüm varlık kümeleri burada toplanır.
    /// </summary>
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, int> // Identity ile entegre DbContext
    {
        /// <summary>
        /// DI üzerinden bağlamı başlatır.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } // Standart DI kurucusu

        // İçerik yönetimi DbSet'leri
        public DbSet<DietitianProfile> DietitianProfiles => Set<DietitianProfile>(); // Diyetisyen profil içerikleri
        public DbSet<BlogPost> BlogPosts => Set<BlogPost>(); // Blog yazıları
        public DbSet<Tag> Tags => Set<Tag>(); // Etiketler
        public DbSet<BlogPostTag> BlogPostTags => Set<BlogPostTag>(); // Blog-etiket köprüleri
        public DbSet<MediaFile> MediaFiles => Set<MediaFile>(); // Medya dosyaları
        public DbSet<BlogPostMedia> BlogPostMedias => Set<BlogPostMedia>(); // Blog-medya eşleşmeleri

        // Zamanlama/randevu DbSet'leri
        public DbSet<WorkingHourException> WorkingHourExceptions => Set<WorkingHourException>(); // İstisna gün/saatler
        public DbSet<Appointment> Appointments => Set<Appointment>(); // Randevular
        public DbSet<AppointmentReminderLog> AppointmentReminderLogs => Set<AppointmentReminderLog>(); // Hatırlatma logları

        // Ayar DbSet'leri
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>(); // Genel sistem ayarları

        /// <summary>
        /// Model oluşturma aşamasında tür dönüştürücüleri, konfigürasyonlar ve global filtreleri uygular.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder) // EF Core model kurulum noktası
        {
            base.OnModelCreating(modelBuilder); // Identity şemasının kurulması için temel çağrı

            // Assembly içindeki tüm IEntityTypeConfiguration<T> sınıflarını otomatik uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly); // Konfigürasyonları yükle

            // Soft-delete uygulayan varlıklar için global sorgu filtresi ata
            modelBuilder.ApplySoftDeleteFilters(); // IsDeleted=true kayıtları otomatik hariç tut
        }
    }
}

