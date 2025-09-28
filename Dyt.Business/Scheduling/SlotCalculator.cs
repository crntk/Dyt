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
    /// Çalışma şablonları, istisnalar ve slot uzunluğuna göre günlük uygun zaman aralıklarını hesaplayan yardımcı sınıf.
    /// Sadece zaman aralığı üretir; mevcut randevuların düşülmesi üst serviste yapılır.
    /// </summary>
    public static class SlotCalculator // Statik yardımcı sınıf olarak tasarladım
    {
        /// <summary>
        /// Verilen gün için aktif şablonlardan slot aralıklarını üretir.
        /// İstisnalarla (kapalı/ekstra açık) şablonlar birleştirilir ve final slot listesi çıkar.
        /// </summary>
        public static List<SlotDto> BuildDailySlots( // Günlük slot üretimini yapan metot
            DateOnly date, // Hesaplama yapılacak gün
            IEnumerable<WorkingHourTemplate> templates, // Haftalık şablonlar
            IEnumerable<WorkingHourException> exceptions) // Günlük istisnalar
        {
            var result = new List<SlotDto>(); // Sonuç listesi için boş bir liste oluşturuyorum

            var dayOfWeek = date.DayOfWeek; // Günün haftanın hangi gününe denk geldiğini alıyorum
            var activeTemplates = templates // Aktif şablonları filtreliyorum
                .Where(t => t.IsActive && t.DayOfWeek == dayOfWeek) // Günle eşleşen ve aktif olanları seçiyorum
                .OrderBy(t => t.StartTime) // Başlangıç saatine göre sıraya koyuyorum
                .ToList(); // Listeye çeviriyorum

            // İlgili güne ait istisnaları alıyorum
            var dayExceptions = exceptions
                .Where(e => e.Date == date) // Sadece bu güne ait olanları seçiyorum
                .ToList(); // Listeye çeviriyorum

            // Eğer tam gün kapalı istisnası varsa, o gün hiç slot üretmem
            if (dayExceptions.Any(x => x.Type == Data.Enums.WorkingExceptionType.Closed && x.StartTime == null && x.EndTime == null)) // Tam gün kapalı kontrolü
                return result; // Boş liste dönerim

            // Şablonlardan temel çalışma aralıklarını çıkarıyorum
            var baseWindows = activeTemplates
                .Select(t => (start: t.StartTime, end: t.EndTime, slot: t.SlotMinutes)) // Başlangıç, bitiş ve slot uzunluğunu alıyorum
                .ToList(); // Liste yapıyorum

            // İstisnalardan ekstra açık aralıklar varsa bunları da ekliyorum
            foreach (var ex in dayExceptions.Where(x => x.Type == Data.Enums.WorkingExceptionType.ExtraOpen)) // Ekstra açık istisnaları geziyorum
            {
                if (ex.StartTime.HasValue && ex.EndTime.HasValue) // Başlangıç ve bitiş saati varsa
                {
                    baseWindows.Add((ex.StartTime.Value, ex.EndTime.Value, // Aralığı şablon listesine ekliyorum
                        activeTemplates.FirstOrDefault()?.SlotMinutes ?? 30)); // Slot süresi için bir şablon varsa onu, yoksa 30 dk alıyorum
                }
            }

            // Kapalı istisnalar belirli saat aralıklarını kapatabilir; bu durumda o aralıklarda slot üretmeyeceğim
            var closedWindows = dayExceptions
                .Where(x => x.Type == Data.Enums.WorkingExceptionType.Closed && x.StartTime.HasValue && x.EndTime.HasValue) // Saat bazlı kapalı istisnalar
                .Select(x => (x.StartTime!.Value, x.EndTime!.Value)) // Başlangıç ve bitiş saatlerini alıyorum
                .ToList(); // Liste yapıyorum

            // Her pencere için slotları parçalıyorum
            foreach (var (start, end, slot) in baseWindows) // Tüm çalışma aralıklarını geziyorum
            {
                var cursor = start; // Slot üretimine başlangıç saatinden başlıyorum
                while (cursor.AddMinutes(slot) <= end) // Slot bitişi pencere bitişini geçmediği sürece devam ediyorum
                {
                    var next = cursor.AddMinutes(slot); // Slotun bitişini hesaplıyorum

                    // Bu slot kapalı istisna aralığına denk geliyor mu kontrol ediyorum
                    var overlapsClosed = closedWindows.Any(cw => // Her kapalı aralıkla çakışmayı kontrol ediyorum
                        !(next <= cw.Item1 || cursor >= cw.Item2)); // Ayrık aralık değilse çakışma vardır

                    if (!overlapsClosed) // Kapalı ile çakışmıyorsa
                    {
                        result.Add(new SlotDto // Yeni bir slot oluşturuyorum
                        {
                            Date = date, // Gün bilgisini set ediyorum
                            StartTime = cursor, // Slot başlangıcı
                            EndTime = next // Slot bitişi
                        }); // Slotu listeye ekliyorum
                    }

                    cursor = next; // Bir sonraki slot için başlangıcı ileri alıyorum
                }
            }

            return result // Üretilen tüm slotları döndürüyorum
                .OrderBy(s => s.StartTime) // Başlangıca göre sıralıyorum
                .ToList(); // Listeye çeviriyorum
        }
    }
}

