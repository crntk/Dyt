using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Contracts.Appointments.Responses
{
    /// <summary>UI'a dönen randevu özeti.</summary>
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string? ClientEmail { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string ConfirmationState { get; set; } = "Yanıtlanmadı";
        public string Channel { get; set; } = string.Empty; // Online | Yüzyüze
        public DateTime CreatedAtUtc { get; set; } // Randevu oluşturma zamanı
    }
}

