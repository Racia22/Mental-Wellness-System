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
    /// Patient Dashboard - shows upcoming appointments and quick actions
    /// </summary>
    [Authorize(Policy = "RequirePatientRole")]
    public class DashboardModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IReportingService _reportingService;

        public DashboardModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            IReportingService reportingService)
        {
            _context = context;
            _userManager = userManager;
            _reportingService = reportingService;
        }

        public ApplicationUser? CurrentUser { get; set; }
        public List<Appointment> UpcomingAppointments { get; set; } = new();
        public PatientDashboardStats Stats { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            CurrentUser = user;

            // Get upcoming appointments (next 30 days)
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var thirtyDaysLater = today.AddDays(30);

            UpcomingAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.PatientId == user.Id
                         && a.AppointmentDate >= today
                         && a.AppointmentDate <= thirtyDaysLater
                         && a.Status != "Cancelled")
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Take(10)
                .ToListAsync();

            // Get statistics using reporting service
            Stats = await _reportingService.GetPatientDashboardStatsAsync(user.Id);

            return Page();
        }
    }
}

