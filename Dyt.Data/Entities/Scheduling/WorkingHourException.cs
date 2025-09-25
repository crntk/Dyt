using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dyt.Data.Entities.Common; // AuditableEntity için
using Dyt.Data.Enums; // WorkingExceptionType enum'u için

namespace Dyt.Data.Entities.Scheduling // Zamanlama varlıkları ad alanı
{
    /// <summary>
    /// Haftalık çalışma şablonuna ek istisnaları (tam gün kapalı, ekstra açık saat) tanımlar.
    /// </summary>
    public class WorkingHourException : AuditableEntity // Audit alanları için kalıtım
    {
        public DateOnly Date { get; set; } // İstisnanın geçerli olduğu gün

        public TimeOnly? StartTime { get; set; } // Kısmi saat aralığı başlangıcı (tam gün ise boş bırakılır)

        public TimeOnly? EndTime { get; set; } // Kısmi saat aralığı bitişi (tam gün ise boş bırakılır)

        public WorkingExceptionType Type { get; set; } // İstisna türü (Kapalı / Ekstra Açık)

        public string? Note { get; set; } // Kısa açıklama/not
    }
}

