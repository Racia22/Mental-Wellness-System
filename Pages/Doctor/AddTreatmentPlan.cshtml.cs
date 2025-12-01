using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;

namespace MentalWellnessSystem.Pages.Doctor
{
    [Authorize(Roles = "Doctor")]
    public class AddTreatmentPlanModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecommendationService _recommendationService;

        public AddTreatmentPlanModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            IRecommendationService recommendationService)
        {
            _context = context;
            _userManager = userManager;
            _recommendationService = recommendationService;
        }

        [BindProperty]
        public TreatmentPlan Input { get; set; } = new();

        public SelectList Patients { get; set; } = null!;
        public SelectList Appointments { get; set; } = null!;

        public async Task OnGetAsync(int? appointmentId = null, int? patientRecordId = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            // Get patients this doctor has seen
            var patientIds = await _context.Appointments
                .Where(a => a.DoctorId == user.Id)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            var patients = await _context.Users
                .Where(u => patientIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync();

            Patients = new SelectList(patients, "Id", "FullName");

            // Get appointments
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == user.Id && a.Status == "Completed")
                .Include(a => a.Patient)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new { a.AppointmentId, Display = $"{a.Patient.FullName} - {a.AppointmentDate:MMM dd, yyyy}" })
                .ToListAsync();

            Appointments = new SelectList(appointments, "AppointmentId", "Display");

            if (appointmentId.HasValue)
            {
                Input.AppointmentId = appointmentId.Value;
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId.Value);
                if (appointment != null)
                {
                    Input.PatientId = appointment.PatientId;
                }
            }

            if (patientRecordId.HasValue)
            {
                Input.PatientRecordId = patientRecordId.Value;
            }

            Input.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
            Input.Status = "Active";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Identity/Account/Login");

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Create new treatment plan
            var treatmentPlan = new TreatmentPlan
            {
                PatientId = Input.PatientId,
                DoctorId = user.Id,
                Title = Input.Title,
                Description = Input.Description,
                Goals = Input.Goals,
                Tasks = Input.Tasks,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate,
                Status = Input.Status ?? "Active",
                AppointmentId = Input.AppointmentId,
                PatientRecordId = Input.PatientRecordId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TreatmentPlans.Add(treatmentPlan);
            await _context.SaveChangesAsync();

            // Generate recommendations based on treatment plan (now that we have the ID)
            try
            {
                await _recommendationService.GenerateRecommendationsFromTreatmentPlanAsync(treatmentPlan.TreatmentPlanId);
            }
            catch
            {
                // Log error but don't fail the creation
            }

            TempData["SuccessMessage"] = "Treatment plan created successfully!";
            return RedirectToPage("/Doctor/TreatmentPlans");
        }
    }
}

