using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dyt.Business.Interfaces.Appointments;
using Dyt.Data.Context;
using Microsoft.EntityFrameworkCore;
using Dyt.Contracts.Appointments.Requests;

namespace Dyt.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NotificationsController : Controller
    {
        private readonly IAppointmentService _appointments;
        private readonly AppDbContext _db;

   public NotificationsController(IAppointmentService appointments, AppDbContext db)
        {
     _appointments = appointments;
         _db = db;
      }

     /// <summary>
        /// Bildirimler ana sayfasý - yeni randevular ve iletiþim mesajlarý
        /// </summary>
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            // Bekleyen randevular (onaylanmamýþ)
        var pendingAppointmentsQuery = new AppointmentQueryRequest
 {
       ConfirmationState = "Yanýtlanmadý",
    PageSize = 50,
         Page = 1
            };
var pendingAppointments = await _appointments.QueryAsync(pendingAppointmentsQuery, ct);

            // Okunmamýþ iletiþim mesajlarý
       var unreadMessages = await _db.ContactMessages
         .Where(m => !m.IsDeleted && !m.IsRead)
            .OrderByDescending(m => m.CreatedAtUtc)
    .Take(50)
    .ToListAsync(ct);

            ViewBag.PendingAppointments = pendingAppointments.Items;
            ViewBag.UnreadMessages = unreadMessages;
       ViewBag.TotalNotifications = pendingAppointments.Items.Count + unreadMessages.Count;

        return View();
     }
    }
}
