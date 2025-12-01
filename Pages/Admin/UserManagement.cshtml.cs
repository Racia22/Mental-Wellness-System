using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Admin
{
    /// <summary>
    /// User Management page - allows admin to view, update, and delete users
    /// </summary>
    [Authorize(Policy = "RequireAdminRole")]
    public class UserManagementModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<UserViewModel> Users { get; set; } = new();

        public class UserViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public bool IsApproved { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            var allUsers = await _context.Users.ToListAsync();
            var usersList = new List<UserViewModel>();

            foreach (var appUser in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(appUser);
                usersList.Add(new UserViewModel
                {
                    Id = appUser.Id,
                    FullName = appUser.FullName,
                    Email = appUser.Email ?? "",
                    Role = roles.FirstOrDefault() ?? "No Role",
                    IsApproved = appUser.IsApproved,
                    CreatedAt = appUser.CreatedAt
                });
            }

            Users = usersList.OrderBy(u => u.CreatedAt).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string userId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            if (userId == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            var userToDelete = await _userManager.FindByIdAsync(userId);
            if (userToDelete == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(userToDelete);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"User {userToDelete.FullName} has been deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting user.";
            }

            return RedirectToPage();
        }
    }
}

