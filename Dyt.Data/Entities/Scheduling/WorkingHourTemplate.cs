using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için

namespace Dyt.Data.Entities.Scheduling // Zamanlama varlıkları ad alanı
{
    /// <summary>
    /// Haftanın her günü için tekrar eden çalışma saatlerini ve slot süresini tanımlar.
    /// </summary>
    public class WorkingHourTemplate : AuditableEntity // Audit ve soft-delete alanlarını kalıt
    {
        public DayOfWeek DayOfWeek { get; set; } // Haftanın günü

        public TimeOnly StartTime { get; set; } // Günün başlangıç saati

        public TimeOnly EndTime { get; set; } // Günün bitiş saati

        public int SlotMinutes { get; set; } = 30; // Randevu slot süresi (dakika)

        public bool IsActive { get; set; } = true; // Bu şablon aktif mi
    }
}

