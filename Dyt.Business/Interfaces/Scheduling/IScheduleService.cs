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
        // Mevcut günlük slot hesaplama imzası zaten burada (dokunmuyorum)

        /// <summary>
        /// Varsayılan slot süresini dakika cinsinden döndürür.
        /// </summary>
        int GetDefaultSlotMinutes(); // Slot süresi için yardımcı metodu bildiriyorum

        /// <summary>
        /// Verilen gün için çalışılabilir slotları hesaplar.
        /// </summary>
        Task<IReadOnlyList<SlotDto>> GetDailySlotsAsync(DateOnly date, CancellationToken ct = default); // Günlük slotları döndüren metodu bildiriyorum

        /// <summary>
        /// Verilen gün için tüm slotlar ve doluluk durumlarını döner.
        /// </summary>
        Task<IReadOnlyList<SlotStateDto>> GetDailySlotStatesAsync(DateOnly date, CancellationToken ct = default);

        /// <summary>
        /// Verilen gün ve saat aralığının uygun olup olmadığını kontrol eder.
        /// </summary>
        Task<bool> IsSlotAvailableAsync(DateOnly date, TimeOnly start, TimeOnly end, CancellationToken ct = default); // Uygunluk kontrol metodunu bildiriyorum

        // Şablon CRUD
        Task<IReadOnlyList<WorkingHourTemplateDto>> GetTemplatesAsync(CancellationToken ct = default); // Tüm şablonları listeler
        Task<int> UpsertTemplateAsync(WorkingHourTemplateUpsertRequest req, CancellationToken ct = default); // Ekle/Güncelle ve Id döner
        Task<bool> DeleteTemplateAsync(int id, CancellationToken ct = default); // Satırı siler

        // İstisna CRUD
        Task<IReadOnlyList<WorkingHourExceptionDto>> GetExceptionsAsync(DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default); // Tarih aralığına göre listeler
        Task<int> UpsertExceptionAsync(WorkingHourExceptionUpsertRequest req, CancellationToken ct = default); // Ekle/Güncelle ve Id döner
        Task<bool> DeleteExceptionAsync(int id, CancellationToken ct = default); // Kaydı siler
    }
}

