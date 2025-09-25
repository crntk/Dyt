using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için
using Microsoft.EntityFrameworkCore; // ChangeTracker erişimi için
using Microsoft.EntityFrameworkCore.Diagnostics; // SaveChangesInterceptor için

namespace Dyt.Data.Interceptors // Interceptor ad alanı
{
    /// <summary>
    /// SaveChanges öncesi CreatedAt/UpdatedAt/DeletedAt alanlarını otomatik dolduran interceptor.
    /// </summary>
    public class SaveChangesAuditingInterceptor : SaveChangesInterceptor // EF interceptor temel sınıfı
    {
        /// <summary>
        /// Senkron kayıt sırasında audit alanlarını damgalar.
        /// </summary>
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) // Override metot
        {
            Stamp(eventData); // Ortak damgalama işlemini çağır
            return base.SavingChanges(eventData, result); // Üst sınıfa devam et
        }

        /// <summary>
        /// Asenkron kayıt sırasında audit alanlarını damgalar.
        /// </summary>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) // Async override
        {
            Stamp(eventData); // Ortak damgalama
            return base.SavingChangesAsync(eventData, result, cancellationToken); // Üst sınıfı çağır
        }

        /// <summary>
        /// Takip edilen varlıkları inceleyip uygun alanları set eder.
        /// </summary>
        private static void Stamp(DbContextEventData eventData) // Yardımcı metot
        {
            if (eventData.Context is not DbContext ctx) return; // Güvenlik kontrolü
            var now = DateTime.UtcNow; // UTC zamanını al

            foreach (var entry in ctx.ChangeTracker.Entries()) // Tüm değişen girdileri dolaş
            {
                if (entry.Entity is BaseEntity baseEntity) // BaseEntity kullanan her nesne için
                {
                    if (entry.State == EntityState.Added) // Yeni kayıt ise
                        baseEntity.CreatedAtUtc = now; // Oluşturulma zamanını ata

                    if (entry.State == EntityState.Modified) // Güncelleme ise
                        baseEntity.UpdatedAtUtc = now; // Güncellenme zamanını ata
                }

                if (entry.Entity is ISoftDelete soft && entry.State == EntityState.Deleted) // Soft-delete uygulanacaksa
                {
                    entry.State = EntityState.Modified; // Fiziksel silme yerine güncelleme moduna al
                    soft.IsDeleted = true; // Silindi işareti
                    soft.DeletedAtUtc = now; // Silinme zamanı
                }
            }
        }
    }
}

