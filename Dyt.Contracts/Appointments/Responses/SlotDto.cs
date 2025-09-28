using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Contracts.Appointments.Responses
{
    /// <summary>Uygunluk listesinde tek bir zaman aralığı.</summary>
    public class SlotDto
    {
        public DateOnly Date { get; set; }         // Gün
        public TimeOnly StartTime { get; set; }    // Başlangıç
        public TimeOnly EndTime { get; set; }      // Bitiş
    }
}
