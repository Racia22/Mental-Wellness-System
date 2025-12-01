using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MentalWellnessSystem.Pages.Admin
{
    /// <summary>
    /// Admin Profile Management page
    /// </summary>
    [Authorize(Policy = "RequireAdminRole")]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public ApplicationUser? CurrentUser { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            [MaxLength(100)]
            public string FullName { get; set; } = string.Empty;

            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string? Phone { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            CurrentUser = user;
            Input.FullName = user.FullName;
            Input.Email = user.Email;
            Input.Phone = user.Phone;

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
                CurrentUser = user;
                return Page();
            }

            // Update full name
            user.FullName = Input.FullName;

            // Update email if changed
            if (!string.IsNullOrEmpty(Input.Email) && Input.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    CurrentUser = user;
                    return Page();
                }
            }

            // Update phone
            if (Input.Phone != user.Phone)
            {
                user.Phone = Input.Phone;
                user.PhoneNumber = Input.Phone;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Profile updated successfully.";
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToPage();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            CurrentUser = user;
            return Page();
        }
    }
}

