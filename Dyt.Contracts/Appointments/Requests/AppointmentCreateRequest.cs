using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyt.Contracts.Appointments.Requests
{
    /// <summary>Randevu oluşturma isteği.</summary>
    public class AppointmentCreateRequest
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;
        public string? ClientEmail { get; set; }
        public string? Note { get; set; }
    }
}

