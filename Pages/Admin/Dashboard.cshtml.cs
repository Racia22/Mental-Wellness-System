using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MentalWellnessSystem.Pages.Admin
{
    /// <summary>
    /// Admin Dashboard - shows system statistics
    /// </summary>
    [Authorize(Policy = "RequireAdminRole")]
    public class DashboardModel : PageModel
    {
        private readonly IReportingService _reportingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardModel(
            IReportingService reportingService,
            UserManager<ApplicationUser> userManager)
        {
            _reportingService = reportingService;
            _userManager = userManager;
        }

        public AdminDashboardStats Stats { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Get statistics using reporting service
            Stats = await _reportingService.GetAdminDashboardStatsAsync();

            return Page();
        }
    }
}

