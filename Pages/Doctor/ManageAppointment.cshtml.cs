using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Doctor
{
    /// <summary>
    /// Manage Appointment page - allows doctors to update appointment status and add notes
    /// </summary>
    [Authorize(Policy = "RequireDoctorRole")]
    public class ManageAppointmentModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageAppointmentModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Appointment? Appointment { get; set; }
        public List<Appointment> AllAppointments { get; set; } = new();

        [BindProperty]
        public string Status { get; set; } = string.Empty;

        [BindProperty]
        public string? Notes { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Get all appointments for this doctor
            AllAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.DoctorId == user.Id)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToListAsync();

            if (id.HasValue)
            {
                Appointment = AllAppointments.FirstOrDefault(a => a.AppointmentId == id.Value);
                if (Appointment != null)
                {
                    Status = Appointment.Status;
                    Notes = Appointment.Notes;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            if (!id.HasValue)
            {
                return RedirectToPage();
            }

            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == id.Value && a.DoctorId == user.Id);

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = Status;
            appointment.Notes = Notes;
            appointment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Appointment updated successfully.";
            return RedirectToPage(new { id = id.Value });
        }
    }
}

