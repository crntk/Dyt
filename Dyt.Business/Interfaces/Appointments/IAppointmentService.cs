using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Contracts.Appointments; // DTO tiplerini kullanmak için ekliyorum
using Dyt.Contracts.Appointments.Requests;   // AppointmentCreateRequest burada
using Dyt.Contracts.Appointments.Responses;  // AppointmentDto, SlotDto burada

namespace Dyt.Business.Interfaces.Appointments // Randevu servis sözleşmelerinin bulunduğu ad alanını tanımlıyorum
{
    /// <summary>
    /// Randevularla ilgili iş kurallarını barındıran servis sözleşmesi.
    /// </summary>
    public interface IAppointmentService // Randevu servisi için arayüz tanımı yapıyorum
    {
        Task<Dyt.Contracts.Appointments.Responses.AppointmentDto>
            CreateAsync(Dyt.Contracts.Appointments.Requests.AppointmentCreateRequest request, CancellationToken ct = default); // Yeni randevu oluşturma imzasını bildiriyorum

        Task<Dyt.Contracts.Appointments.Responses.AppointmentDto?>
            GetByIdAsync(int id, CancellationToken ct = default); // Kimliğe göre randevu dönen imzayı bildiriyorum

        Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotDto>>
            GetAvailableSlotsAsync(DateOnly date, CancellationToken ct = default); // Verilen gün için uygun slotları dönen imzayı bildiriyorum

        Task<bool> MarkStatusNoShowAsync(int id, CancellationToken ct = default); // Gelmedi olarak işaretleme imzasını bildiriyorum

        Task<bool> CancelAsync(int id, CancellationToken ct = default); // Randevu iptal imzasını bildiriyorum

        Task<bool> ProcessConfirmationAsync(string token, string intent, CancellationToken ct = default); // Onay linkindeki token ve niyeti işleyen yeni imzayı ekliyorum
    }
}




