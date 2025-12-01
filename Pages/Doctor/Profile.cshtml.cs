using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Doctor
{
    /// <summary>
    /// Doctor Profile Management page
    /// </summary>
    [Authorize(Policy = "RequireDoctorRole")]
    public class ProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly MentalWellnessDbContext _context;

        public ProfileModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            MentalWellnessDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public ApplicationUser? CurrentUser { get; set; }
        public List<Feedback> Reviews { get; set; } = new();

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

            [Display(Name = "Specialty")]
            [MaxLength(100)]
            public string? Specialty { get; set; }
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
            Input.Specialty = user.Specialty;

            // Get reviews for this doctor
            Reviews = await _context.Feedbacks
                .Include(f => f.Patient)
                .Include(f => f.Appointment)
                .Where(f => f.DoctorId == user.Id)
                .OrderByDescending(f => f.SubmittedAt)
                .ToListAsync();

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

            // Update specialty
            user.Specialty = Input.Specialty;

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

