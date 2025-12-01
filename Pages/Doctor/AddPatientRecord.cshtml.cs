using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Doctor
{
    
    [Authorize(Policy = "RequireDoctorRole")]
    public class AddPatientRecordModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddPatientRecordModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? PatientId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? AppointmentId { get; set; }

        public ApplicationUser? Patient { get; set; }
        public Appointment? Appointment { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Session Notes")]
            public string Notes { get; set; } = string.Empty;

            [Display(Name = "Diagnosis")]
            [MaxLength(500)]
            public string? Diagnosis { get; set; }

            [Display(Name = "Follow-up Request")]
            [MaxLength(500)]
            public string? FollowUpRequest { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string? patientId, int? appointmentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            if (appointmentId.HasValue)
            {
                Appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId.Value && a.DoctorId == user.Id);

                if (Appointment != null)
                {
                    Patient = Appointment.Patient;
                    PatientId = Patient.Id;
                }
            }
            else if (!string.IsNullOrEmpty(patientId))
            {
                Patient = await _userManager.FindByIdAsync(patientId);
            }

            if (Patient == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsApproved)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(PatientId))
                {
                    Patient = await _userManager.FindByIdAsync(PatientId);
                }
                if (AppointmentId.HasValue)
                {
                    Appointment = await _context.Appointments
                        .Include(a => a.Patient)
                        .FirstOrDefaultAsync(a => a.AppointmentId == AppointmentId.Value);
                }
                return Page();
            }

            if (string.IsNullOrEmpty(PatientId))
            {
                ModelState.AddModelError(string.Empty, "Patient ID is required.");
                return Page();
            }

            var record = new PatientRecord
            {
                PatientId = PatientId,
                DoctorId = user.Id,
                Notes = Input.Notes,
                Diagnosis = Input.Diagnosis,
                FollowUpRequest = Input.FollowUpRequest,
                AppointmentId = AppointmentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.PatientRecords.Add(record);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient record added successfully.";
            return RedirectToPage("/Doctor/PatientRecords", new { patientId = PatientId });
        }
    }
}

