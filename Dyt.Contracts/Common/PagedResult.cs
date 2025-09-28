using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Contracts.Common
{
    /// <summary>
    /// Sayfalı sonuçlar için genel amaçlı kapsayıcı.
    /// </summary>
    public class PagedResult<T> // GENERIC tip
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>(); // Sonuç listesi
        public int TotalCount { get; init; }                              // Toplam kayıt
        public int Page { get; init; }                                    // Mevcut sayfa
        public int PageSize { get; init; }                                // Sayfa boyutu
    }
}

