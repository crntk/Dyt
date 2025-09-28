using Dyt.Business.Interfaces.Scheduling;
using Dyt.Business.Scheduling;
using Dyt.Business.Utils;
using Dyt.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Dyt.Business.Services.Scheduling
{
    /// <summary>
    /// Çalışma şablonları, istisnalar ve mevcut randevulara göre uygun slotları hesaplar.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext _db;
        private readonly IDateTimeProvider _clock;

        public ScheduleService(AppDbContext db, IDateTimeProvider clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotDto>>
            GetDailySlotsAsync(DateOnly date, CancellationToken ct = default)
        {
            var templates = await _db.WorkingHourTemplates.AsNoTracking().ToListAsync(ct);
            var exceptions = await _db.WorkingHourExceptions.AsNoTracking().Where(e => e.Date == date).ToListAsync(ct);

            var rawSlots = SlotCalculator.BuildDailySlots(
                date,
                templates,
                exceptions
            );

            var appointments = await _db.Appointments
                .AsNoTracking()
                .Where(a => a.AppointmentDate == date)
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync(ct);

            var available = rawSlots.Where(s => !appointments.Any(a => !(s.EndTime <= a.StartTime || s.StartTime >= a.EndTime))).ToList();

            if (date == _clock.TodayUtc)
            {
                var nowTime = TimeOnly.FromDateTime(_clock.UtcNow);
                available = available.Where(s => s.StartTime > nowTime).ToList();
            }

            return available;
        }

        public async Task<bool> IsSlotAvailableAsync(DateOnly date, TimeOnly start, TimeOnly end, CancellationToken ct = default)
        {
            var slots = await GetDailySlotsAsync(date, ct);
            return slots.Any(s => s.StartTime == start && s.EndTime == end);
        }

        public int GetDefaultSlotMinutes()
        {
            var template = _db.WorkingHourTemplates.AsNoTracking().FirstOrDefault(t => t.IsActive);
            return template?.SlotMinutes ?? 30;
        }
    }
}
