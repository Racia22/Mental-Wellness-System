using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Doctor
{
    /// <summary>
    /// Patient Records page - shows patient history and records
    /// </summary>
    [Authorize(Policy = "RequireDoctorRole")]
    public class PatientRecordsModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;

        public PatientRecordsModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuditService auditService)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
        }

        public ApplicationUser? Patient { get; set; }
        public List<PatientRecord> Records { get; set; } = new();
        public List<Appointment> Appointments { get; set; } = new();
        public List<ApplicationUser> AllPatients { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? PatientId { get; set; }

        public async Task<IActionResult> OnGetAsync(string? patientId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Get all patients this doctor has appointments with
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == user.Id)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            AllPatients = await _context.Users
                .Where(u => patientIds.Contains(u.Id))
                .OrderBy(u => u.FullName)
                .ToListAsync();

            if (!string.IsNullOrEmpty(patientId))
            {
                Patient = await _userManager.FindByIdAsync(patientId);
                if (Patient == null)
                {
                    return NotFound();
                }

                // Log access to patient records
                await _auditService.LogPatientRecordAccessAsync(user.Id, 0, "View", HttpContext.Connection.RemoteIpAddress?.ToString());

                // Get patient records created by this doctor
                Records = await _context.PatientRecords
                    .Include(r => r.Doctor)
                    .Include(r => r.Appointment)
                    .Where(r => r.PatientId == patientId && r.DoctorId == user.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                // Get patient appointments with this doctor
                Appointments = await _context.Appointments
                    .Where(a => a.PatientId == patientId && a.DoctorId == user.Id)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .ToListAsync();
            }

            return Page();
        }
    }
}

