using Dyt.Contracts.Appointments;
using Dyt.Contracts.Appointments.Responses;
using Dyt.Contracts.Scheduling.Requests;  // Upsert istekleri için ekliyorum
using Dyt.Contracts.Scheduling.Responses; // DTO’lar için ekliyorum
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Interfaces.Scheduling // Arayüzler için ad alanı
{
    /// <summary>
    /// Çalışma saatleri ve istisnaları yöneten servis sözleşmesi.
    /// </summary>
    public interface IScheduleService // Arayüz tanımı
    {
        /// <summary>
        /// Varsayılan slot süresini dakika cinsinden döndürür.
        /// </summary>
        int GetDefaultSlotMinutes();

        /// <summary>
        /// Verilen gün için çalışılabilir slotları hesaplar.
        /// </summary>
        Task<IReadOnlyList<SlotDto>> GetDailySlotsAsync(DateOnly date, CancellationToken ct = default);

        /// <summary>
        /// Verilen gün için tüm slotlar ve doluluk durumlarını döner.
        /// </summary>
        Task<IReadOnlyList<SlotStateDto>> GetDailySlotStatesAsync(DateOnly date, CancellationToken ct = default);

        /// <summary>
        /// Verilen gün ve saat aralığının uygun olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsSlotAvailableAsync(DateOnly date, TimeOnly start, TimeOnly end, CancellationToken ct = default);

        // İstisna CRUD
        Task<IReadOnlyList<Dyt.Contracts.Scheduling.Responses.WorkingHourExceptionDto>> GetExceptionsAsync(DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default);
        Task<int> UpsertExceptionAsync(Dyt.Contracts.Scheduling.Requests.WorkingHourExceptionUpsertRequest req, CancellationToken ct = default);
        Task<bool> DeleteExceptionAsync(int id, CancellationToken ct = default);

        // Slot yönetimi
        Task<int> CloseSlotsAsync(DateOnly date, IEnumerable<TimeOnly> startTimes, CancellationToken ct = default);
        Task<bool> UpdateClosedSlotsForDateAsync(DateOnly date, IEnumerable<TimeOnly> startTimes, CancellationToken ct = default);
        Task<IReadOnlyList<TimeOnly>> GetClosedSlotStartsAsync(DateOnly date, CancellationToken ct = default);
        Task<IReadOnlyList<TimeOnly>> GetReservedSlotStartsAsync(DateOnly date, bool onlyConfirmed = true, CancellationToken ct = default);
        Task<int> OpenSlotsAsync(DateOnly date, IEnumerable<TimeOnly> startTimes, CancellationToken ct = default);
    }
}

