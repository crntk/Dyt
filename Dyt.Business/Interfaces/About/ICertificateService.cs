using Dyt.Contracts.About.Requests;
using Dyt.Contracts.About.Responses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.About
{
    /// <summary>
    /// Sertifika/Baþarý yönetimi için interface
    /// </summary>
  public interface ICertificateService
    {
        /// <summary>
     /// Tüm yayýnlanmýþ sertifikalarý listeler (sýralý)
        /// </summary>
 Task<List<CertificateDto>> GetAllPublishedAsync(CancellationToken ct = default);

    /// <summary>
 /// ID'ye göre sertifika getirir
  /// </summary>
        Task<CertificateDto?> GetByIdAsync(int id, CancellationToken ct = default);

     /// <summary>
  /// Yeni sertifika oluþturur
        /// </summary>
 Task<CertificateDto> CreateAsync(CertificateRequest request, CancellationToken ct = default);

 /// <summary>
        /// Sertifika günceller
        /// </summary>
  Task<CertificateDto?> UpdateAsync(int id, CertificateRequest request, CancellationToken ct = default);

   /// <summary>
  /// Sertifika siler (soft delete)
   /// </summary>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
