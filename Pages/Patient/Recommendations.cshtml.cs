using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;

namespace MentalWellnessSystem.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class RecommendationsModel : PageModel
    {
        private readonly IRecommendationService _recommendationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RecommendationsModel(
            IRecommendationService recommendationService,
            UserManager<ApplicationUser> userManager)
        {
            _recommendationService = recommendationService;
            _userManager = userManager;
        }

        public List<ResourceRecommendation> Recommendations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            // Get only recommendations that are based on treatment plans (from doctors)
            // This ensures recommendations are only from treatment plans, not mood logs
            var allRecommendations = await _recommendationService.GetPatientRecommendationsAsync(user.Id);
            Recommendations = allRecommendations.Where(r => r.TreatmentPlanId.HasValue).ToList();
        }

        public async Task<IActionResult> OnPostMarkViewedAsync(int recommendationId)
        {
            await _recommendationService.MarkAsViewedAsync(recommendationId);
            return RedirectToPage();
        }
    }
}

