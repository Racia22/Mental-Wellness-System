using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Doctor
{
    /// <summary>
    /// Doctor Dashboard - shows appointments filtered by time period
    /// </summary>
    [Authorize(Policy = "RequireDoctorRole")]
    public class DashboardModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ApplicationUser? CurrentUser { get; set; }
        public List<Appointment> Appointments { get; set; } = new();
        public List<Feedback> PatientFeedbacks { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "Day"; // Day, Week, Month, Year

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            CurrentUser = user;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.DoctorId == user.Id)
                .AsQueryable();

            // Apply time period filter
            switch (Filter)
            {
                case "Day":
                    query = query.Where(a => a.AppointmentDate == today);
                    break;
                case "Week":
                    var weekEnd = today.AddDays(7);
                    query = query.Where(a => a.AppointmentDate >= today && a.AppointmentDate < weekEnd);
                    break;
                case "Month":
                    var monthEnd = today.AddMonths(1);
                    query = query.Where(a => a.AppointmentDate >= today && a.AppointmentDate < monthEnd);
                    break;
                case "Year":
                    var yearEnd = today.AddYears(1);
                    query = query.Where(a => a.AppointmentDate >= today && a.AppointmentDate < yearEnd);
                    break;
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(StatusFilter))
            {
                query = query.Where(a => a.Status == StatusFilter);
            }

            Appointments = await query
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();

            // Get patient feedbacks for this doctor (all feedbacks, not just 10)
            PatientFeedbacks = await _context.Feedbacks
                .Include(f => f.Patient)
                .Include(f => f.Appointment)
                .Where(f => f.DoctorId == user.Id)
                .OrderByDescending(f => f.SubmittedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int appointmentId, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.DoctorId == user.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment status updated successfully.";
            return RedirectToPage();
        }
    }
}

