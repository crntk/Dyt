using Dyt.Contracts.Appointments; // SlotDto dönüş tipini kullanmak için
using Dyt.Contracts.Appointments.Responses;
using Dyt.Data.Entities.Scheduling; // Çalışma saatleri ve istisna varlıklarını kullanmak için
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Business.Scheduling // Slot hesaplayıcısının bulunduğu ad alanı
{
    /// <summary>
    /// Tarih bazlı istisnalar ve slot uzunluğuna göre günlük uygun zaman aralıklarını hesaplayan yardımcı sınıf.
    /// Haftalık şablonlar kaldırıldı; yalnızca tarih bazlı istisnalar (ExtraOpen/Closed) dikkate alınır.
    /// </summary>
    public static class SlotCalculator // Statik yardımcı sınıf olarak tasarladım
    {
        /// <summary>
        /// Verilen gün için tarih bazlı istisnalardan slot aralıklarını üretir.
        /// - ExtraOpen kayıtları çalışma pencerelerini tanımlar.
        /// - Closed kayıtları bu pencerelerden düşülür.
        /// </summary>
        public static List<SlotDto> BuildDailySlots(
            DateOnly date,
            IEnumerable<WorkingHourException> exceptions)
        {
            var result = new List<SlotDto>();

            // İlgili güne ait istisnaları alıyorum
            var dayExceptions = exceptions
                .Where(e => e.Date == date)
                .ToList();

            // Eğer tam gün kapalı istisnası varsa, slot üretme
            if (dayExceptions.Any(x => x.Type == Data.Enums.WorkingExceptionType.Closed && x.StartTime == null && x.EndTime == null))
                return result;

            // ExtraOpen pencereleri: yalnızca bunlardan üretim yapılır
            var baseWindows = dayExceptions
                .Where(x => x.Type == Data.Enums.WorkingExceptionType.ExtraOpen && x.StartTime.HasValue && x.EndTime.HasValue)
                .Select(x => (start: x.StartTime!.Value, end: x.EndTime!.Value, slot: 30)) // Varsayılan slot: 30 dk
                .ToList();

            if (baseWindows.Count == 0)
                return result; // O gün için hiç açık aralık yoksa boş liste dön

            // Saat bazlı kapalı aralıklar
            var closedWindows = dayExceptions
                .Where(x => x.Type == Data.Enums.WorkingExceptionType.Closed && x.StartTime.HasValue && x.EndTime.HasValue)
                .Select(x => (x.StartTime!.Value, x.EndTime!.Value))
                .ToList();

            foreach (var (start, end, slot) in baseWindows)
            {
                var cursor = start;
                while (cursor.AddMinutes(slot) <= end)
                {
                    var next = cursor.AddMinutes(slot);
                    var overlapsClosed = closedWindows.Any(cw => !(next <= cw.Item1 || cursor >= cw.Item2));
                    if (!overlapsClosed)
                    {
                        result.Add(new SlotDto
                        {
                            Date = date,
                            StartTime = cursor,
                            EndTime = next
                        });
                    }
                    cursor = next;
                }
            }

            return result.OrderBy(s => s.StartTime).ToList();
        }

        /// <summary>
        /// Slot durumları için ham slot listesi üretir. Kapalı istisnaları düşmez; yalnızca ExtraOpen pencereleri slotlara ayrılır.
        /// Böylece kapatılmış saatler de liste içinde yer alır (durumu UI/servis belirler).
        /// </summary>
        public static List<SlotDto> BuildDailySlotsRaw(
            DateOnly date,
            IEnumerable<WorkingHourException> exceptions)
        {
            var result = new List<SlotDto>();

            var dayExceptions = exceptions.Where(e => e.Date == date).ToList();

            // Tam gün kapalı ise yine de slot üretmeyelim (tamamen kapalı gün)
            if (dayExceptions.Any(x => x.Type == Data.Enums.WorkingExceptionType.Closed && x.StartTime == null && x.EndTime == null))
                return result;

            var baseWindows = dayExceptions
                .Where(x => x.Type == Data.Enums.WorkingExceptionType.ExtraOpen && x.StartTime.HasValue && x.EndTime.HasValue)
                .Select(x => (start: x.StartTime!.Value, end: x.EndTime!.Value, slot: 30))
                .ToList();

            if (baseWindows.Count == 0)
                return result;

            foreach (var (start, end, slot) in baseWindows)
            {
                var cursor = start;
                while (cursor.AddMinutes(slot) <= end)
                {
                    var next = cursor.AddMinutes(slot);
                    result.Add(new SlotDto { Date = date, StartTime = cursor, EndTime = next });
                    cursor = next;
                }
            }
            return result.OrderBy(s => s.StartTime).ToList();
        }
    }
}

