using System.Threading.Tasks;
using Dyt.Business.Interfaces.Appointments;
using Dyt.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Dyt.Web.ViewComponents
{
    [ViewComponent(Name = "AdminNotifications")]
    public class AdminNotificationsViewComponent : ViewComponent
    {
        private readonly IAppointmentService _appointments;
        private readonly AppDbContext _db;
        
        public AdminNotificationsViewComponent(IAppointmentService appointments, AppDbContext db)
 {
 _appointments = appointments;
   _db = db;
  }

        public async Task<IViewComponentResult> InvokeAsync()
        {
   // Sadece admin oturumunda göster
      if (!(User?.Identity?.IsAuthenticated ?? false))
    return View("Default", 0);

     // Bekleyen randevular
    var pendingAppointments = await _appointments.GetPendingCountAsync();
      
   // Okunmamýþ iletiþim mesajlarý
   var unreadMessages = await _db.ContactMessages
      .Where(m => !m.IsDeleted && !m.IsRead)
                .CountAsync();
            
            // Toplam bildirim sayýsý
        var totalCount = pendingAppointments + unreadMessages;
    
            return View("Default", totalCount);
        }
    }
}
