using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Admin
{
    /// <summary>
    /// Approve Doctors page - allows admin to approve pending doctor registrations
    /// </summary>
    [Authorize(Policy = "RequireAdminRole")]
    public class ApproveDoctorsModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApproveDoctorsModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<ApplicationUser> PendingDoctors { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            // Get all doctors who are not approved
            var doctorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Doctor");
            if (doctorRole != null)
            {
                var doctorUserIds = await _context.UserRoles
                    .Where(ur => ur.RoleId == doctorRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                PendingDoctors = await _context.Users
                    .Where(u => doctorUserIds.Contains(u.Id) && !u.IsApproved)
                    .OrderBy(u => u.CreatedAt)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var doctor = await _userManager.FindByIdAsync(userId);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.IsApproved = true;
            var result = await _userManager.UpdateAsync(doctor);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Doctor {doctor.FullName} has been approved.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error approving doctor.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var doctor = await _userManager.FindByIdAsync(userId);
            if (doctor == null)
            {
                return NotFound();
            }

            // Optionally delete the user or just mark as rejected
            var result = await _userManager.DeleteAsync(doctor);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Doctor {doctor.FullName} has been rejected and removed.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error rejecting doctor.";
            }

            return RedirectToPage();
        }
    }
}

