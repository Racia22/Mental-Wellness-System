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
    /// View Appointment page - shows appointment details and allows PDF download
    /// </summary>
    [Authorize(Policy = "RequirePatientRole")]
    public class ViewAppointmentModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPdfService _pdfService;
        private readonly INotificationService _notificationService;

        public ViewAppointmentModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            IPdfService pdfService,
            INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _pdfService = pdfService;
            _notificationService = notificationService;
        }

        public Appointment? Appointment { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            Appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PatientId == user.Id);

            if (Appointment == null)
            {
                return NotFound();
            }

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
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
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

            appointment.Status = "Cancelled";
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Send cancellation notification
            await _notificationService.SendCancellationNotificationAsync(appointment, "Patient");

            TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            return RedirectToPage(new { id = id });
        }

        public async Task<IActionResult> OnGetDownloadPdfAsync(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PatientId == user.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            try
            {
                var pdfBytes = await _pdfService.GenerateAppointmentPdfAsync(id);
                // Return as HTML file that can be saved and opened in any browser
                // User can print to PDF using browser's print function
                Response.Headers.Add("Content-Disposition", $"attachment; filename=Appointment_{id}.html");
                return File(pdfBytes, "text/html; charset=utf-8", $"Appointment_{id}.html");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Error generating document.";
                return RedirectToPage(new { id = id });
            }
        }
    }
}

