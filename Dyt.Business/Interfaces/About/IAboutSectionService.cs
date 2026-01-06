using Dyt.Contracts.About.Requests;
using Dyt.Contracts.About.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.About
{
    /// <summary>
    /// "Ben Kimim" bölümü için interface
    /// </summary>
    public interface IAboutSectionService
    {
        /// <summary>
        /// Aktif "Ben Kimim" içeriðini getirir (sadece yayýnda olanlar - frontend için)
        /// </summary>
        Task<AboutSectionDto?> GetActiveAsync(CancellationToken ct = default);

        /// <summary>
        /// Mevcut "Ben Kimim" içeriðini getirir (yayýnda olsun ya da olmasýn - admin için)
        /// </summary>
        Task<AboutSectionDto?> GetCurrentAsync(CancellationToken ct = default);

        /// <summary>
        /// "Ben Kimim" içeriðini oluþturur veya günceller
        /// </summary>
        Task<AboutSectionDto> UpsertAsync(AboutSectionRequest request, CancellationToken ct = default);

        /// <summary>
        /// "Ben Kimim" içeriðini siler
        /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
