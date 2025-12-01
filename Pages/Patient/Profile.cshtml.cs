using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MentalWellnessSystem.Pages.Patient
{
    /// <summary>
    /// Patient Profile Management page
    /// </summary>
    [Authorize(Policy = "RequirePatientRole")]
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
            [EmailAddress]
            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string? Phone { get; set; }

            [Display(Name = "Age")]
            [Range(1, 120)]
            public int? Age { get; set; }

            [Display(Name = "Gender")]
            public string? Gender { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            CurrentUser = user;
            Input.Email = user.Email;
            Input.Phone = user.Phone;
            Input.Age = user.Age;
            Input.Gender = user.Gender;

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

            // Update age and gender (PatientID cannot be changed)
            user.Age = Input.Age;
            user.Gender = Input.Gender;

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

