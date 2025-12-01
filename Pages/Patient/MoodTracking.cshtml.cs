using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;

namespace MentalWellnessSystem.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class MoodTrackingModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRecommendationService _recommendationService;

        public MoodTrackingModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            IRecommendationService recommendationService)
        {
            _context = context;
            _userManager = userManager;
            _recommendationService = recommendationService;
        }

        [BindProperty]
        public MoodLog Input { get; set; } = new();

        public List<MoodLog> RecentLogs { get; set; } = new();
        public List<ResourceRecommendation> Recommendations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            RecentLogs = await _context.MoodLogs
                .Where(m => m.PatientId == user.Id)
                .OrderByDescending(m => m.LogDate)
                .Take(30)
                .ToListAsync();

            Recommendations = await _recommendationService.GetPatientRecommendationsAsync(user.Id, unviewedOnly: true);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Identity/Account/Login");

            // Check if log already exists for this date
            var existingLog = await _context.MoodLogs
                .FirstOrDefaultAsync(m => m.PatientId == user.Id && m.LogDate == Input.LogDate);

            if (existingLog != null)
            {
                // Update existing log
                existingLog.MoodScore = Input.MoodScore;
                existingLog.StressLevel = Input.StressLevel;
                existingLog.SleepHours = Input.SleepHours;
                existingLog.EnergyLevel = Input.EnergyLevel;
                existingLog.Notes = Input.Notes;
            }
            else
            {
                // Create new log
                Input.PatientId = user.Id;
                Input.CreatedAt = DateTime.UtcNow;
                _context.MoodLogs.Add(Input);
            }

            await _context.SaveChangesAsync();

            // Generate recommendations if mood is low
            if (Input.MoodScore < 5)
            {
                await _recommendationService.GenerateRecommendationsFromMoodLogsAsync(user.Id);
            }

            TempData["SuccessMessage"] = "Mood log saved successfully!";
            return RedirectToPage();
        }
    }
}

