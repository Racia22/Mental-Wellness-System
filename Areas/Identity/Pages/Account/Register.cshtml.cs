using System.ComponentModel.DataAnnotations;
using MentalWellnessSystem.Models;
using MentalWellnessSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MentalWellnessSystem.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Registration page model with role-based user creation
    /// </summary>
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPatientIDGeneratorService _patientIDGenerator;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IPatientIDGeneratorService patientIDGenerator,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _patientIDGenerator = patientIDGenerator;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full name")]
            public string FullName { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Phone Number")]
            [Phone]
            public string? Phone { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required]
            [Display(Name = "Role")]
            public string Role { get; set; } = string.Empty; // Admin, Doctor, Patient

            [Display(Name = "Therapy Type / Specialization")]
            public string? TherapyType { get; set; } // For Patients/Doctors

            // Additional fields for patients
            [Display(Name = "Age")]
            [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
            public int? Age { get; set; }

            [Display(Name = "Gender")]
            public string? Gender { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate role
            if (Input.Role != "Patient" && Input.Role != "Doctor" && Input.Role != "Admin")
            {
                ModelState.AddModelError(string.Empty, "Invalid role selected.");
                return Page();
            }

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmailConfirmed = false, // Set to true if email confirmation is disabled
                FullName = Input.FullName,
                Phone = Input.Phone,
                PhoneNumber = Input.Phone,
                CreatedAt = DateTime.UtcNow
            };

            // Set role-specific properties
            if (Input.Role == "Patient")
            {
                // Generate PatientID
                user.PatientID = await _patientIDGenerator.GeneratePatientIDAsync();
                user.Category = Input.TherapyType;
                user.Age = Input.Age;
                user.Gender = Input.Gender;
                user.IsApproved = true; // Patients are auto-approved
            }
            else if (Input.Role == "Doctor")
            {
                user.Specialty = Input.TherapyType;
                user.IsApproved = false; // Requires admin approval
            }
            else if (Input.Role == "Admin")
            {
                user.IsApproved = true; // Admins are auto-approved
            }

            // Create user
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password: {Email}", Input.Email);

                // Add user to role
                if (!await _roleManager.RoleExistsAsync(Input.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Input.Role));
                }

                await _userManager.AddToRoleAsync(user, Input.Role);

                _logger.LogInformation("User {Email} added to role {Role}", Input.Email, Input.Role);

                // For patients, show PatientID
                if (Input.Role == "Patient" && !string.IsNullOrEmpty(user.PatientID))
                {
                    TempData["PatientID"] = user.PatientID;
                    TempData["SuccessMessage"] = $"Registration successful! Your Patient ID is: {user.PatientID}";
                }
                else if (Input.Role == "Doctor")
                {
                    TempData["SuccessMessage"] = "Registration successful! Your account is pending admin approval. You will be notified once approved.";
                }
                else
                {
                    TempData["SuccessMessage"] = "Registration successful!";
                }

                // Redirect to login
                return RedirectToPage("./Login", new { returnUrl = ReturnUrl });
            }

            // If Identity creation failed, show errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
