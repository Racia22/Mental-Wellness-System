using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Pages.Patient
{
    [Authorize(Roles = "Patient")]
    public class TreatmentPlansModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TreatmentPlansModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<TreatmentPlan> TreatmentPlans { get; set; } = new();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return;

            TreatmentPlans = await _context.TreatmentPlans
                .Include(t => t.Doctor)
                .Include(t => t.Appointment)
                .Where(t => t.PatientId == user.Id)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
    }
}
