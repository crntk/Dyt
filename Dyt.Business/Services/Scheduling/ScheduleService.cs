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

            // Yerel saate göre (sunucunun yerel saati) bugünün geçmiş slotlarını gizle
            var nowLocal = _clock.UtcNow.ToLocalTime();
            var todayLocal = DateOnly.FromDateTime(nowLocal);
            if (date == todayLocal)
            {
                var nowTime = TimeOnly.FromDateTime(nowLocal);
                available = available.Where(s => s.StartTime > nowTime).ToList();
            }

            return available;
        }

        public async Task<IReadOnlyList<Dyt.Contracts.Appointments.Responses.SlotStateDto>> GetDailySlotStatesAsync(DateOnly date, CancellationToken ct = default)
        {
            // Ham slotları üret (kapalılar dahil)
            var exceptions = await _db.WorkingHourExceptions.AsNoTracking().Where(e => e.Date == date).ToListAsync(ct);
            var rawSlots = SlotCalculator.BuildDailySlotsRaw(date, exceptions);

            // Kapalı istisnalar
            var closedWindows = exceptions
                .Where(x => x.Type == WorkingExceptionType.Closed && x.StartTime.HasValue && x.EndTime.HasValue)
                .Select(x => (x.StartTime!.Value, x.EndTime!.Value))
                .ToList();

            // Randevular
            var appointments = await _db.Appointments.AsNoTracking()
                .Where(a => a.AppointmentDate == date)
                .Select(a => new { a.StartTime, a.EndTime })
                .ToListAsync(ct);

            var list = new List<Dyt.Contracts.Appointments.Responses.SlotStateDto>(rawSlots.Count);
            foreach (var s in rawSlots)
            {
                var isClosed = closedWindows.Any(cw => !(s.EndTime <= cw.Item1 || s.StartTime >= cw.Item2));
                var isBusy = appointments.Any(a => !(s.EndTime <= a.StartTime || s.StartTime >= a.EndTime));
                var isAvailable = !isClosed && !isBusy;

                // Bugün geçmiş saatleri müsait saymayalım
                var nowLocal = _clock.UtcNow.ToLocalTime();
                var todayLocal = DateOnly.FromDateTime(nowLocal);
                if (date == todayLocal)
                {
                    var nowTime = TimeOnly.FromDateTime(nowLocal);
                    if (s.StartTime <= nowTime) isAvailable = false;
                }

                list.Add(new Dyt.Contracts.Appointments.Responses.SlotStateDto
                {
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsAvailable = isAvailable
                });
            }
            return list.OrderBy(x => x.StartTime).ToList();
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

        // Yeni: slot kapatma/açma yönetimi
        public async Task<int> CloseSlotsAsync(DateOnly date, IEnumerable<TimeOnly> startTimes, CancellationToken ct = default)
        {
            var list = startTimes?.Distinct().OrderBy(t => t).ToList() ?? new List<TimeOnly>();
            if (list.Count == 0) return 0;

            var slotMinutes = GetDefaultSlotMinutes();
            var endTimes = list.Select(s => s.AddMinutes(slotMinutes)).ToList();

            // O günün mevcut saat-bazlı Closed kayıtlarını getir (soft-deleted olmayan)
            var existing = await _db.WorkingHourExceptions
                .Where(e => e.Date == date && e.Type == WorkingExceptionType.Closed && e.StartTime != null && e.EndTime != null && !e.IsDeleted)
                .ToListAsync(ct);

            int added = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var s = list[i];
                var e = endTimes[i];
                var exists = existing.Any(x => x.StartTime == s && x.EndTime == e);
                if (!exists)
                {
                    var entity = new WorkingHourException
                    {
                        Date = date,
                        StartTime = s,
                        EndTime = e,
                        Type = WorkingExceptionType.Closed,
                        Note = "Admin - toplu kapatma",
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    await _db.WorkingHourExceptions.AddAsync(entity, ct);
                    added++;
                }
            }
            if (added > 0)
                await _db.SaveChangesAsync(ct);
            return added;
        }

        public async Task<bool> UpdateClosedSlotsForDateAsync(DateOnly date, IEnumerable<TimeOnly> startTimes, CancellationToken ct = default)
        {
            var desired = startTimes?.Distinct().OrderBy(t => t).ToList() ?? new List<TimeOnly>();
            var slotMinutes = GetDefaultSlotMinutes();

            var existing = await _db.WorkingHourExceptions
                .Where(e => e.Date == date && e.Type == WorkingExceptionType.Closed && e.StartTime != null && e.EndTime != null && !e.IsDeleted)
                .ToListAsync(ct);

            // Silinecekler: mevcut olup desired içinde olmayanlar
            var toDelete = existing.Where(x => !desired.Any(d => x.StartTime == d && x.EndTime == d.AddMinutes(slotMinutes))).ToList();
            foreach (var ex in toDelete)
            {
                ex.IsDeleted = true;
                ex.DeletedAtUtc = DateTime.UtcNow;
                _db.WorkingHourExceptions.Update(ex);
            }

            // Eklenecekler: desired olup mevcutta olmayanlar
            foreach (var d in desired)
            {
                var e = d.AddMinutes(slotMinutes);
                var exists = existing.Any(x => x.StartTime == d && x.EndTime == e && !x.IsDeleted);
                if (!exists)
                {
                    var entity = new WorkingHourException
                    {
                        Date = date,
                        StartTime = d,
                        EndTime = e,
                        Type = WorkingExceptionType.Closed,
                        Note = "Admin - güncelle",
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    await _db.WorkingHourExceptions.AddAsync(entity, ct);
                }
            }

            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<IReadOnlyList<TimeOnly>> GetClosedSlotStartsAsync(DateOnly date, CancellationToken ct = default)
        {
            var list = await _db.WorkingHourExceptions
                .AsNoTracking()
                .Where(e => e.Date == date && e.Type == WorkingExceptionType.Closed && e.StartTime != null && e.EndTime != null && !e.IsDeleted)
                .OrderBy(e => e.StartTime)
                .Select(e => e.StartTime!.Value)
                .ToListAsync(ct);
            return list;
        }

        public async Task<IReadOnlyList<TimeOnly>> GetReservedSlotStartsAsync(DateOnly date, bool onlyConfirmed = true, CancellationToken ct = default)
        {
            var q = _db.Appointments.AsNoTracking().Where(a => a.AppointmentDate == date && a.Status == AppointmentStatus.Scheduled);
            if (onlyConfirmed)
                q = q.Where(a => a.ConfirmationState == ConfirmationState.Gelecek);
            return await q.OrderBy(a => a.StartTime).Select(a => a.StartTime).ToListAsync(ct);
        }

        public async Task<int> OpenSlotsAsync(DateOnly date, IEnumerable<TimeOnly> startTimes, CancellationToken ct = default)
        {
            var list = startTimes?.Distinct().OrderBy(t => t).ToList() ?? new List<TimeOnly>();
            if (list.Count == 0) return 0;
            var slotMinutes = GetDefaultSlotMinutes();

            var existing = await _db.WorkingHourExceptions
                .Where(e => e.Date == date && e.Type == WorkingExceptionType.Closed && e.StartTime != null && e.EndTime != null && !e.IsDeleted)
                .ToListAsync(ct);

            int affected = 0;
            foreach (var s in list)
            {
                var e = s.AddMinutes(slotMinutes);
                var ex = existing.FirstOrDefault(x => x.StartTime == s && x.EndTime == e);
                if (ex != null)
                {
                    ex.IsDeleted = true;
                    ex.DeletedAtUtc = DateTime.UtcNow;
                    _db.WorkingHourExceptions.Update(ex);
                    affected++;
                }
            }
            if (affected > 0)
                await _db.SaveChangesAsync(ct);
            return affected;
        }
    }
}
