using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Pages.Community
{
    [Authorize]
    public class CreatePostModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreatePostModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public CommunityPost Input { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToPage("/Identity/Account/Login");

            Input.UserId = user.Id;
            Input.CreatedAt = DateTime.UtcNow;
            Input.UpdatedAt = DateTime.UtcNow;
            Input.Category = Input.Category ?? "General";

            _context.CommunityPosts.Add(Input);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Post created successfully!";
            return RedirectToPage("/Community/Index");
        }
    }
}

