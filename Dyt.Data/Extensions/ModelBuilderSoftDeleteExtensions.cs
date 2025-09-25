using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions; // Expression ağaçlarıyla dinamik filtre kurmak için
using Dyt.Data.Entities.Common; // ISoftDelete için
using Microsoft.EntityFrameworkCore; // ModelBuilder için

namespace Dyt.Data.Extensions // Model oluşturma uzantılarının tutulduğu ad alanı
{
    /// <summary>
    /// Soft-delete uygulayan varlıklar için global sorgu filtresi ekleyen uzantılar.
    /// Bu sayede IsDeleted=true kayıtlar otomatik olarak dışarıda kalır.
    /// </summary>
    public static class ModelBuilderSoftDeleteExtensions // ModelBuilder'a uzantı metotlarını sağlayan statik sınıf
    {
        /// <summary>
        /// ISoftDelete arayüzünü uygulayan tüm varlıklara global filtre uygular.
        /// </summary>
        public static void ApplySoftDeleteFilters(this ModelBuilder mb) // ModelBuilder için genişletme metodu
        {
            foreach (var entityType in mb.Model.GetEntityTypes()) // Modeldeki tüm varlık türlerini dolaş
            {
                var clr = entityType.ClrType; // Türün CLR karşılığını al
                if (typeof(ISoftDelete).IsAssignableFrom(clr)) // Tür ISoftDelete'i uyguluyor mu kontrol et
                {
                    var parameter = Expression.Parameter(clr, "e"); // e parametresi (entity)
                    var prop = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)); // e.IsDeleted ifadesi
                    var condition = Expression.Equal(prop, Expression.Constant(false)); // e.IsDeleted == false koşulu
                    var lambda = Expression.Lambda(condition, parameter); // Lambda ifadeyi derle

                    mb.Entity(clr).HasQueryFilter(lambda); // İlgili türe global sorgu filtresi ata
                }
            }
        }
    }
}

