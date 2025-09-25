using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Data.Repositories // Repository ad alanı
{
    /// <summary>
    /// Birden fazla repository işlemini tek transaction altında yönetmek için birim iş arayüzü.
    /// </summary>
    public interface IUnitOfWork // Basit UoW arayüzü
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default); // Değişiklikleri tek noktadan kaydet
    }
}

