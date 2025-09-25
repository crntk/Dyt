using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions; // Include ifadeleri için
using Dyt.Data.Context; // AppDbContext için
using Dyt.Data.Entities.Common; // BaseEntity için
using Microsoft.EntityFrameworkCore; // EF çekirdek

namespace Dyt.Data.Repositories // Repository ad alanı
{
    /// <summary>
    /// EF Core tabanlı genel amaçlı repository implementasyonu.
    /// </summary>
    public class EfRepository<T> : IRepository<T> where T : BaseEntity // Generic EF repository
    {
        private readonly AppDbContext _db; // DbContext bağımlılığı
        private readonly DbSet<T> _set; // T için DbSet referansı

        public EfRepository(AppDbContext db) // Kurucu
        {
            _db = db; // DbContext ata
            _set = db.Set<T>(); // DbSet ata
        }

        public IQueryable<T> Query(params Expression<Func<T, object>>[] includes) // Sorgu başlat
        {
            IQueryable<T> q = _set.AsQueryable(); // Temel sorgu
            foreach (var inc in includes) // Tüm include ifadelerini uygula
                q = q.Include(inc); // Include ekle
            return q; // Sorguyu döndür
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) // Tek kayıt getir
            => await _set.FirstOrDefaultAsync(x => x.Id == id, ct); // Id eşleşeni getir

        public async Task AddAsync(T entity, CancellationToken ct = default) // Ekleme
            => await _set.AddAsync(entity, ct); // EF'ye ekle

        public void Update(T entity) // Güncelleme
            => _set.Update(entity); // EF state'i Modified yapar

        public void Remove(T entity) // Silme
            => _set.Remove(entity); // Interceptor sayesinde soft-delete'e dönüşür

        public Task<int> SaveChangesAsync(CancellationToken ct = default) // Kaydet
            => _db.SaveChangesAsync(ct); // DbContext'e delege et
    }
}

