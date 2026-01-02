using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dyt.Data.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dyt.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactMessagesController : Controller
    {
  private readonly AppDbContext _db;

        public ContactMessagesController(AppDbContext db)
  {
            _db = db;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
var messages = await _db.ContactMessages
  .OrderByDescending(m => m.CreatedAtUtc)
  .ToListAsync(ct);

       return View(messages);
    }

     [HttpGet]
public async Task<IActionResult> Details(int id, CancellationToken ct)
      {
            var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
  if (message == null)
     return NotFound();

        // Mesajý okundu olarak iþaretle
            if (!message.IsRead)
   {
   message.IsRead = true;
          message.ReadAt = DateTime.UtcNow;
    await _db.SaveChangesAsync(ct);
         }

          return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
     public async Task<IActionResult> ToggleRead(int id, CancellationToken ct)
     {
   var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
  if (message == null)
       return NotFound();

  message.IsRead = !message.IsRead;
         message.ReadAt = message.IsRead ? DateTime.UtcNow : null;
            await _db.SaveChangesAsync(ct);

    TempData["SuccessMessage"] = message.IsRead 
       ? "Mesaj okundu olarak iþaretlendi." 
        : "Mesaj okunmadý olarak iþaretlendi.";

         return RedirectToAction(nameof(Index));
        }

        [HttpPost]
[ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReplied(int id, CancellationToken ct)
     {
     var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
            if (message == null)
       return NotFound();

            message.IsReplied = !message.IsReplied;
            message.RepliedAt = message.IsReplied ? DateTime.UtcNow : null;
   await _db.SaveChangesAsync(ct);

   TempData["SuccessMessage"] = message.IsReplied 
     ? "Mesaj yanýtlandý olarak iþaretlendi." 
     : "Mesaj yanýtlanmadý olarak iþaretlendi.";

        return RedirectToAction(nameof(Details), new { id });
   }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNote(int id, string? adminNote, CancellationToken ct)
        {
     var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
   if (message == null)
      return NotFound();

            message.AdminNote = adminNote;
    await _db.SaveChangesAsync(ct);

            TempData["SuccessMessage"] = "Not güncellendi.";
     return RedirectToAction(nameof(Details), new { id });
        }

 [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
 {
            var message = await _db.ContactMessages.FindAsync(new object[] { id }, ct);
   if (message == null)
return NotFound();

      message.IsDeleted = true;
    await _db.SaveChangesAsync(ct);

            TempData["SuccessMessage"] = "Mesaj silindi.";
       return RedirectToAction(nameof(Index));
}
    }
}
