using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Contracts.Appointments;
using Dyt.Contracts.Appointments.Responses; // SlotDto'yu kullanmak için ekliyorum

namespace Dyt.Business.Interfaces.Scheduling // Zamanlama/uygunluk servisleri için ad alanını tanımlıyorum
{
    /// <summary>
    /// Çalışma şablonları, istisnalar ve mevcut randevulara göre uygun slot üretiminden sorumlu servis.
    /// </summary>
    public interface IScheduleService // Uygunluk hizmeti arayüzü
    {
        Task<IReadOnlyList<SlotDto>> GetDailySlotsAsync(DateOnly date, CancellationToken ct = default); // Bir gün için uygun slotları hesaplayan metodu bildiriyorum
        Task<bool> IsSlotAvailableAsync(DateOnly date, TimeOnly start, TimeOnly end, CancellationToken ct = default); // Belirli bir aralığın uygun olup olmadığını kontrol eden metodu bildiriyorum
        int GetDefaultSlotMinutes(); // Varsayılan slot süresini (dakika) dönen metodu bildiriyorum
    }
}

