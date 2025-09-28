using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Contracts.Profile; // Profil DTO'larını kullanmak için ekliyorum
using Dyt.Contracts.Profile.Requests;   // ProfileUpdateRequest burada
using Dyt.Contracts.Profile.Responses;  // DietitianProfileDto burada

namespace Dyt.Business.Interfaces.Profile
{
    /// <summary>
    /// Ana sayfa profil içeriklerinin okunması ve yönetilmesinden sorumlu servis.
    /// </summary>
    public interface IProfileService
    {
        Task<DietitianProfileDto?> GetAsync(CancellationToken ct = default);            // Profili getir
        Task<bool> UpdateAsync(ProfileUpdateRequest request, CancellationToken ct = default); // Profili güncelle
    }
}


