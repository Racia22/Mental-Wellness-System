using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class FeedbackModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Feedback Input { get; set; } = new();

        public SelectList CompletedAppointments { get; set; } = null!;
        public SelectList DoctorsList { get; set; } = null!;
        public List<Feedback> MyFeedbacks { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            // Get all doctors this patient has had appointments with
            var doctorIds = await _context.Appointments
                .Where(a => a.PatientId == user.Id && a.Status == "Completed")
                .Select(a => a.DoctorId)
                .Distinct()
                .ToListAsync();

            var doctors = await _context.Users
                .Where(u => doctorIds.Contains(u.Id))
                .Select(d => new { 
                    d.Id, 
                    Display = $"Dr. {d.FullName}" 
                })
                .OrderBy(d => d.Display)
                .ToListAsync();

            DoctorsList = new SelectList(doctors, "Id", "Display");

            // Get completed appointments for this patient (excluding those with feedback)
            var appointmentsQuery = _context.Appointments
                .Where(a => a.PatientId == user.Id && a.Status == "Completed")
                .Include(a => a.Doctor)
                .Where(a => !_context.Feedbacks.Any(f => f.AppointmentId == a.AppointmentId));

            var appointments = await appointmentsQuery
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new { 
                    a.AppointmentId,
                    a.DoctorId,
                    Display = $"Dr. {a.Doctor.FullName} - {a.AppointmentDate:MMM dd, yyyy}" 
                })
                .ToListAsync();

            CompletedAppointments = new SelectList(appointments, "AppointmentId", "Display");

            // Get existing feedbacks
            MyFeedbacks = await _context.Feedbacks
                .Include(f => f.Doctor)
                .Include(f => f.Appointment)
                .Where(f => f.PatientId == user.Id)
                .OrderByDescending(f => f.SubmittedAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Identity/Account/Login");

            // Validate required fields
            if (Input.AppointmentId == 0)
            {
                ModelState.AddModelError("Input.AppointmentId", "Please select an appointment.");
            }

            if (Input.Rating < 1 || Input.Rating > 5)
            {
                ModelState.AddModelError("Input.Rating", "Please select a rating.");
            }

            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            // Get appointment to get doctor ID
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == Input.AppointmentId);

            if (appointment == null)
            {
                ModelState.AddModelError("", "Invalid appointment selected.");
                await OnGetAsync();
                return Page();
            }

            // Check if feedback already exists
            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.AppointmentId == Input.AppointmentId);

            if (existingFeedback != null)
            {
                ModelState.AddModelError("", "Feedback already submitted for this appointment.");
                await OnGetAsync();
                return Page();
            }

            // Create new feedback
            var feedback = new Feedback
            {
                PatientId = user.Id,
                DoctorId = appointment.DoctorId,
                AppointmentId = Input.AppointmentId,
                Rating = Input.Rating,
                Comments = Input.Comments,
                IsAnonymous = Input.IsAnonymous,
                SubmittedAt = DateTime.UtcNow
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Review submitted successfully. Thank you for your feedback!";
            return RedirectToPage();
        }
    }
}

