using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Patient
{
    /// <summary>
    /// Appointment History page - shows all past and current appointments
    /// </summary>
    [Authorize(Policy = "RequirePatientRole")]
    public class AppointmentHistoryModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public AppointmentHistoryModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public List<Appointment> Appointments { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            Appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == user.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PatientId == user.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            if (appointment.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Appointment is already cancelled.";
                return RedirectToPage();
            }

            if (appointment.Status == "Completed")
            {
                TempData["ErrorMessage"] = "Cannot cancel a completed appointment.";
                return RedirectToPage();
            }

            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send cancellation notification
            await _notificationService.SendCancellationNotificationAsync(appointment, "Patient");

            TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            return RedirectToPage();
        }
    }
}

