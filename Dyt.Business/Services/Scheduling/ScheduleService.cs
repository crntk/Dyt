using Dyt.Business.Interfaces.Scheduling;
using Dyt.Business.Scheduling;
using Dyt.Business.Utils;
using Dyt.Data.Context;
using Dyt.Data.Entities.Scheduling;
using Dyt.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dyt.Business.Services.Scheduling
{
    /// <summary>
    /// Tarih bazlı istisnalar ve mevcut randevulara göre uygun slotları hesaplar ve yönetim CRUD işlemlerini sağlar.
    /// Haftalık şablonlar kaldırıldı.
    /// </summary>
    public partial class ScheduleService : IScheduleService
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
            var exceptions = await _db.WorkingHourExceptions.AsNoTracking().Where(e => e.Date == date).ToListAsync(ct);

            var rawSlots = SlotCalculator.BuildDailySlots(
                date,
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

        public async Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotStateDto>> GetDailySlotStatesAsync(DateOnly date, CancellationToken ct = default)
        {
            var available = await GetDailySlotsAsync(date, ct);

            // SlotCalculator artık sadece exceptions ile çalışıyor
            var exceptions = await _db.WorkingHourExceptions.AsNoTracking().Where(e => e.Date == date).ToListAsync(ct);
            var rawSlots = SlotCalculator.BuildDailySlots(date, exceptions);

            var list = new List<Dyt.Contracts.Appointments.Responses.SlotStateDto>(rawSlots.Count);
            foreach (var s in rawSlots)
            {
                list.Add(new Dyt.Contracts.Appointments.Responses.SlotStateDto
                {
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsAvailable = available.Any(a => a.StartTime == s.StartTime && a.EndTime == s.EndTime)
                });
            }
            return list;
        }

        public async Task<bool> IsSlotAvailableAsync(DateOnly date, TimeOnly start, TimeOnly end, CancellationToken ct = default)
        {
            var slots = await GetDailySlotsAsync(date, ct);
            return slots.Any(s => s.StartTime == start && s.EndTime == end);
        }

        public int GetDefaultSlotMinutes()
        {
            // Haftalık şablon kaldırıldı; varsayılan 30 dk
            return 30;
        }

        // Yönetim CRUD: Şablonlar (kullanımdan kaldırıldı)
        public Task<IReadOnlyList<Dyt.Contracts.Scheduling.Responses.WorkingHourTemplateDto>> GetTemplatesAsync(CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<Dyt.Contracts.Scheduling.Responses.WorkingHourTemplateDto>>(new List<Dyt.Contracts.Scheduling.Responses.WorkingHourTemplateDto>());
        public Task<int> UpsertTemplateAsync(Dyt.Contracts.Scheduling.Requests.WorkingHourTemplateUpsertRequest req, CancellationToken ct = default)
            => Task.FromResult(0);
        public Task<bool> DeleteTemplateAsync(int id, CancellationToken ct = default)
            => Task.FromResult(false);

        // Yönetim CRUD: İstisnalar
        public async Task<IReadOnlyList<Dyt.Contracts.Scheduling.Responses.WorkingHourExceptionDto>> GetExceptionsAsync(DateOnly? from = null, DateOnly? to = null, CancellationToken ct = default)
        {
            var q = _db.WorkingHourExceptions.AsNoTracking().AsQueryable();
            if (from.HasValue) q = q.Where(e => e.Date >= from.Value);
            if (to.HasValue) q = q.Where(e => e.Date <= to.Value);

            var list = await q.OrderBy(e => e.Date).ThenBy(e => e.StartTime)
                .Select(e => new Dyt.Contracts.Scheduling.Responses.WorkingHourExceptionDto
                {
                    Id = e.Id,
                    Date = e.Date,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    IsClosed = e.Type == WorkingExceptionType.Closed,
                    Note = e.Note
                })
                .ToListAsync(ct);
            return list;
        }

        public async Task<int> UpsertExceptionAsync(Dyt.Contracts.Scheduling.Requests.WorkingHourExceptionUpsertRequest req, CancellationToken ct = default)
        {
            WorkingHourException entity;
            var type = req.IsClosed ? WorkingExceptionType.Closed : WorkingExceptionType.ExtraOpen;

            if (req.Id.HasValue && req.Id.Value > 0)
            {
                entity = await _db.WorkingHourExceptions.FirstOrDefaultAsync(x => x.Id == req.Id.Value, ct)
                         ?? throw new InvalidOperationException("İstisna kaydı bulunamadı.");
                entity.Date = req.Date;
                entity.StartTime = req.StartTime;
                entity.EndTime = req.EndTime;
                entity.Type = type;
                entity.Note = req.Note;
                entity.UpdatedAtUtc = DateTime.UtcNow;
                _db.WorkingHourExceptions.Update(entity);
            }
            else
            {
                entity = new WorkingHourException
                {
                    Date = req.Date,
                    StartTime = req.StartTime,
                    EndTime = req.EndTime,
                    Type = type,
                    Note = req.Note,
                    CreatedAtUtc = DateTime.UtcNow
                };
                await _db.WorkingHourExceptions.AddAsync(entity, ct);
            }
            await _db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task<bool> DeleteExceptionAsync(int id, CancellationToken ct = default)
        {
            var entity = await _db.WorkingHourExceptions.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            _db.WorkingHourExceptions.Update(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
