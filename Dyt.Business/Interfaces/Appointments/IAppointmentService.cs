using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Contracts.Appointments; // DTO tiplerini kullanmak için ekliyorum
using Dyt.Contracts.Appointments.Requests;   // AppointmentCreateRequest burada
using Dyt.Contracts.Appointments.Responses;  // AppointmentDto, SlotDto burada
using Dyt.Data.Enums; // ConfirmationState için

namespace Dyt.Business.Interfaces.Appointments // Arayüzlerin tutulduğu ad alanını koruyorum
{
    /// <summary>
    /// Randevularla ilgili iş kurallarını barındıran servis sözleşmesi.
    /// </summary>
    public interface IAppointmentService // Servis arayüzünü tanımlıyorum
    {
        Task<Dyt.Contracts.Appointments.Responses.AppointmentDto>
            CreateAsync(Dyt.Contracts.Appointments.Requests.AppointmentCreateRequest request, CancellationToken ct = default); // Oluşturma imzasını bildiriyorum

        Task<Dyt.Contracts.Appointments.Responses.AppointmentDto?>
            GetByIdAsync(int id, CancellationToken ct = default); // Tekil getirme imzasını bildiriyorum

        Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotDto>>
            GetAvailableSlotsAsync(DateOnly date, CancellationToken ct = default); // Günlük slotları dönen imzayı bildiriyorum

        // Yeni: slot state
        Task<IReadOnlyList<SlotStateDto>> GetSlotStatesAsync(DateOnly date, CancellationToken ct = default);

        Task<bool> MarkStatusNoShowAsync(int id, CancellationToken ct = default); // Gelmedi işaretleme imzasını bildiriyorum
        Task<bool> CancelAsync(int id, CancellationToken ct = default); // İptal imzasını bildiriyorum

        Task<bool> ProcessConfirmationAsync(string token, string intent, CancellationToken ct = default); // Onay token işleme imzasını bildiriyorum

        Task<Dyt.Contracts.Common.PagedResult<Dyt.Contracts.Appointments.Responses.AppointmentDto>>
            QueryAsync(Dyt.Contracts.Appointments.Requests.AppointmentQueryRequest request, CancellationToken ct = default); // Admin listeleme için sayfalı sorgu imzasını ekliyorum

        // Yeni: Admin onay/red ve bekleyen sayısı
        Task<bool> AdminSetConfirmationAsync(int id, ConfirmationState state, CancellationToken ct = default);
        Task<int> GetPendingCountAsync(CancellationToken ct = default);
    }
}









