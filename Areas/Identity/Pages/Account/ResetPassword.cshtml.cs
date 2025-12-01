using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace MentalWellnessSystem.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(
            UserManager<ApplicationUser> userManager,
            ILogger<ResetPasswordModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "OTP is required")]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
            [Display(Name = "OTP Code")]
            public string OTP { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm New Password")]
            [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            // Check if OTP session exists
            var storedOTP = HttpContext.Session.GetString("ResetPasswordOTP");
            var userId = HttpContext.Session.GetString("ResetPasswordUserId");
            var expiryStr = HttpContext.Session.GetString("ResetPasswordExpiry");

            if (string.IsNullOrEmpty(storedOTP) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(expiryStr))
            {
                TempData["ErrorMessage"] = "Invalid or expired reset session. Please request a new OTP.";
                return RedirectToPage("./ForgotPassword");
            }

            // Check if OTP has expired
            if (DateTime.TryParse(expiryStr, out var expiry) && DateTime.UtcNow > expiry)
            {
                TempData["ErrorMessage"] = "OTP has expired. Please request a new one.";
                HttpContext.Session.Remove("ResetPasswordOTP");
                HttpContext.Session.Remove("ResetPasswordUserId");
                HttpContext.Session.Remove("ResetPasswordExpiry");
                return RedirectToPage("./ForgotPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve OTP from session
            var storedOTP = HttpContext.Session.GetString("ResetPasswordOTP");
            var userId = HttpContext.Session.GetString("ResetPasswordUserId");
            var expiryStr = HttpContext.Session.GetString("ResetPasswordExpiry");

            if (string.IsNullOrEmpty(storedOTP) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(expiryStr))
            {
                TempData["ErrorMessage"] = "Invalid or expired reset session. Please request a new OTP.";
                return RedirectToPage("./ForgotPassword");
            }

            // Check if OTP has expired
            if (DateTime.TryParse(expiryStr, out var expiry) && DateTime.UtcNow > expiry)
            {
                TempData["ErrorMessage"] = "OTP has expired. Please request a new one.";
                HttpContext.Session.Remove("ResetPasswordOTP");
                HttpContext.Session.Remove("ResetPasswordUserId");
                HttpContext.Session.Remove("ResetPasswordExpiry");
                return RedirectToPage("./ForgotPassword");
            }

            // Verify OTP
            if (Input.OTP != storedOTP)
            {
                ModelState.AddModelError(string.Empty, "Invalid OTP. Please check and try again.");
                return Page();
            }

            // Find user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please request a new OTP.";
                HttpContext.Session.Remove("ResetPasswordOTP");
                HttpContext.Session.Remove("ResetPasswordUserId");
                HttpContext.Session.Remove("ResetPasswordExpiry");
                return RedirectToPage("./ForgotPassword");
            }

            // Reset password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, Input.NewPassword);

            if (result.Succeeded)
            {
                // Clear session
                HttpContext.Session.Remove("ResetPasswordOTP");
                HttpContext.Session.Remove("ResetPasswordUserId");
                HttpContext.Session.Remove("ResetPasswordExpiry");

                _logger.LogInformation("Password reset successful for user: {UserId}", user.Id);
                TempData["SuccessMessage"] = "Password has been reset successfully. You can now login with your new password.";
                return RedirectToPage("./Login");
            }

            // If password reset failed, show errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}

