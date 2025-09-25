using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions; // Sorgu ifadeleri için
using Dyt.Data.Entities.Common; // BaseEntity için
using Microsoft.EntityFrameworkCore; // EF türleri için

namespace Dyt.Data.Repositories // Repository ad alanı
{
    /// <summary>
    /// Temel CRUD işlemleri için genel amaca yönelik repository arayüzü.
    /// Bu projede zorunlu değil; Business doğrudan DbContext kullanabilir.
    /// </summary>
    public interface IRepository<T> where T : BaseEntity // Kısıt: BaseEntity türevi
    {
        IQueryable<T> Query(params Expression<Func<T, object>>[] includes); // Dahil edilecek navigasyonlarla sorgu başlat
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default); // Kimliğe göre tek kayıt getir
        Task AddAsync(T entity, CancellationToken ct = default); // Yeni kayıt ekle
        void Update(T entity); // Kayıt güncelle
        void Remove(T entity); // Soft-delete ile kaldır (interceptor uygulayacak)
        Task<int> SaveChangesAsync(CancellationToken ct = default); // Değişiklikleri kaydet
    }
}

