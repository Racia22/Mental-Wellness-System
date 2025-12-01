using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Patient
{
    /// <summary>
    /// Book Appointment page - allows patients to book appointments with doctors
    /// </summary>
    [Authorize(Policy = "RequirePatientRole")]
    public class BookAppointmentModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<BookAppointmentModel> _logger;

        public BookAppointmentModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            IAppointmentService appointmentService,
            ILogger<BookAppointmentModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public List<SelectListItem> Doctors { get; set; } = new();
        public List<SelectListItem> TimeSlots { get; set; } = new();

        public class InputModel
        {
            [Required]
            [Display(Name = "Doctor")]
            public string DoctorId { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Appointment Date")]
            [DataType(DataType.Date)]
            public DateOnly AppointmentDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

            [Required]
            [Display(Name = "Appointment Time")]
            public string AppointmentTime { get; set; } = string.Empty;

            [Display(Name = "Appointment Type")]
            [MaxLength(50)]
            public string? AppointmentType { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            await LoadDoctorsAsync();
            LoadTimeSlots();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            if (!ModelState.IsValid)
            {
                await LoadDoctorsAsync();
                LoadTimeSlots();
                return Page();
            }

            // Parse time
            if (!TimeOnly.TryParse(Input.AppointmentTime, out var time))
            {
                ModelState.AddModelError(nameof(Input.AppointmentTime), "Invalid time format.");
                await LoadDoctorsAsync();
                LoadTimeSlots();
                return Page();
            }

            // Validate date (cannot book in the past)
            if (Input.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError(nameof(Input.AppointmentDate), "Cannot book appointments in the past.");
                await LoadDoctorsAsync();
                LoadTimeSlots();
                return Page();
            }

            try
            {
                // Check if doctor is approved
                var doctor = await _userManager.FindByIdAsync(Input.DoctorId);
                if (doctor == null || !doctor.IsApproved)
                {
                    ModelState.AddModelError(string.Empty, "Selected doctor is not available.");
                    await LoadDoctorsAsync();
                    LoadTimeSlots();
                    return Page();
                }

                // Book appointment
                var appointment = await _appointmentService.BookAppointmentAsync(
                    user.Id,
                    Input.DoctorId,
                    Input.AppointmentDate,
                    time,
                    Input.AppointmentType);

                TempData["SuccessMessage"] = "Appointment booked successfully!";
                return RedirectToPage("/Patient/ViewAppointment", new { id = appointment.AppointmentId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDoctorsAsync();
                LoadTimeSlots();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking appointment");
                ModelState.AddModelError(string.Empty, "An error occurred while booking the appointment. Please try again.");
                await LoadDoctorsAsync();
                LoadTimeSlots();
                return Page();
            }
        }

        private async Task LoadDoctorsAsync()
        {
            // Get all approved doctors
            var doctors = await _context.Users
                .Where(u => u.IsApproved && _context.UserRoles.Any(ur => ur.UserId == u.Id && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Doctor")))
                .OrderBy(u => u.FullName)
                .ToListAsync();

            Doctors = doctors.Select(d => new SelectListItem
            {
                Value = d.Id,
                Text = $"{d.FullName} - {d.Specialty ?? "General"}",
            }).ToList();
        }

        private void LoadTimeSlots()
        {
            // Generate time slots (9 AM to 5 PM, 30-minute intervals)
            var slots = new List<SelectListItem>();
            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(17, 0);

            while (startTime <= endTime)
            {
                slots.Add(new SelectListItem
                {
                    Value = startTime.ToString("HH:mm"),
                    Text = startTime.ToString("hh:mm tt")
                });
                startTime = startTime.AddMinutes(30);
            }

            TimeSlots = slots;
        }
    }
}

