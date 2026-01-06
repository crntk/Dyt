using Dyt.Contracts.About.Requests;
using Dyt.Contracts.About.Responses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.About
{
    /// <summary>
    /// Deneyim/Özgeçmiþ yönetimi için interface
/// </summary>
    public interface IExperienceService
    {
 /// <summary>
     /// Tüm aktif deneyimleri listeler (sýralý)
   /// </summary>
        Task<List<ExperienceDto>> GetAllAsync(CancellationToken ct = default);

  /// <summary>
        /// ID'ye göre deneyim getirir
        /// </summary>
     Task<ExperienceDto?> GetByIdAsync(int id, CancellationToken ct = default);

      /// <summary>
        /// Yeni deneyim oluþturur
        /// </summary>
  Task<ExperienceDto> CreateAsync(ExperienceRequest request, CancellationToken ct = default);

        /// <summary>
     /// Deneyim günceller
        /// </summary>
 Task<ExperienceDto?> UpdateAsync(int id, ExperienceRequest request, CancellationToken ct = default);

     /// <summary>
  /// Deneyim siler
        /// </summary>
 Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
