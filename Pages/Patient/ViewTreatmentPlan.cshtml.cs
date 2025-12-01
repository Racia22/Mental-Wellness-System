using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class ViewTreatmentPlanModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ViewTreatmentPlanModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public TreatmentPlan? TreatmentPlan { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Identity/Account/Login");

            if (!id.HasValue)
            {
                return NotFound();
            }

            TreatmentPlan = await _context.TreatmentPlans
                .Include(t => t.Patient)
                .Include(t => t.Doctor)
                .Include(t => t.Appointment)
                .FirstOrDefaultAsync(t => t.TreatmentPlanId == id.Value && t.PatientId == user.Id);

            if (TreatmentPlan == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
