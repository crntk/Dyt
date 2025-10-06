using System;

namespace Dyt.Contracts.Appointments.Responses
{
    /// <summary>
    /// Günlük slotun durumunu temsil eder: müsait mi dolu mu.
    /// </summary>
    public class SlotStateDto
    {
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsAvailable { get; set; }
    }
}
