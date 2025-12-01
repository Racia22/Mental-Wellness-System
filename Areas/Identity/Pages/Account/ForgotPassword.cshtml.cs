using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace MentalWellnessSystem.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<ForgotPasswordModel> _logger;

        public ForgotPasswordModel(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ILogger<ForgotPasswordModel> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Find user by username
            var user = await _userManager.FindByNameAsync(Input.Username);
            
            // Verify email matches
            if (user == null || user.Email?.ToLower() != Input.Email.ToLower())
            {
                TempData["ErrorMessage"] = "Invalid username or email. Please check your credentials.";
                return Page();
            }

            // Generate OTP (6-digit code)
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            var otpExpiry = DateTime.UtcNow.AddMinutes(10); // OTP valid for 10 minutes

            // Store OTP in session
            HttpContext.Session.SetString("ResetPasswordOTP", otp);
            HttpContext.Session.SetString("ResetPasswordUserId", user.Id);
            HttpContext.Session.SetString("ResetPasswordExpiry", otpExpiry.ToString("O"));

            // Send OTP via email
            var subject = "Password Reset OTP - Mental Wellness System";
            var body = $@"
Dear {user.FullName},

You have requested to reset your password. Please use the following OTP to proceed:

OTP: {otp}

This OTP will expire in 10 minutes.

If you did not request this password reset, please ignore this email.

Best regards,
Mental Wellness System
";

            var emailSent = await _emailService.SendEmailAsync(user.Email!, subject, body);

            if (emailSent)
            {
                TempData["SuccessMessage"] = "OTP has been sent to your email address. Please check your inbox.";
                _logger.LogInformation("Password reset OTP sent to user: {UserId}", user.Id);
            }
            else
            {
                // For environments where SMTP is not configured, still allow reset flow
                // and surface the OTP so testing can proceed.
                TempData["SuccessMessage"] = $"Email could not be sent, but your OTP for testing is: {otp}";
                _logger.LogWarning("Failed to send password reset OTP to user: {UserId}. OTP for testing: {Otp}", user.Id, otp);
            }

            return RedirectToPage("./ResetPassword");
        }
    }
}

