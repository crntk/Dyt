using System.Threading.Tasks;
using Dyt.Business.Interfaces.Appointments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dyt.Web.ViewComponents
{
    [ViewComponent(Name = "AdminNotifications")]
    public class AdminNotificationsViewComponent : ViewComponent
    {
        private readonly IAppointmentService _appointments;
        public AdminNotificationsViewComponent(IAppointmentService appointments)
        {
            _appointments = appointments;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Sadece admin oturumunda göster
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return View("Default", 0);

            var count = await _appointments.GetPendingCountAsync();
            return View("Default", count);
        }
    }
}
