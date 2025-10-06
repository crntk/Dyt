using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Contracts.Scheduling.Responses // Scheduling yanıt modelleri için ad alanı
{
    /// <summary>
    /// Haftalık çalışma saatleri şablonundaki tek bir satırı temsil eder.
    /// </summary>
    public class WorkingHourTemplateDto // DTO sınıfını tanımlıyorum
    {
        public int Id { get; set; } // Kayıt kimliğini tutuyorum
        public int DayOfWeek { get; set; } // Haftanın gününü (0=Pazar … 6=Cumartesi) tutuyorum
        public TimeOnly StartTime { get; set; } // Başlangıç saatini tutuyorum
        public TimeOnly EndTime { get; set; } // Bitiş saatini tutuyorum
        public bool IsActive { get; set; } // Satırın aktif olup olmadığını tutuyorum
    }
}

