using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Admin
{
    /// <summary>
    /// View User Profile page - allows admin to view user profiles and download documents
    /// </summary>
    [Authorize(Policy = "RequireAdminRole")]
    public class ViewUserProfileModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ViewUserProfileModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public UserProfileViewModel? UserProfile { get; set; }
        public List<PatientDocument> UserDocuments { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? UserId { get; set; }

        public class UserProfileViewModel
        {
            public string Id { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string Role { get; set; } = string.Empty;
            public bool IsApproved { get; set; }
            public string? Specialty { get; set; }
            public string? PatientID { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            if (string.IsNullOrEmpty(UserId))
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "No Role";

            UserProfile = new UserProfileViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = userRole,
                IsApproved = user.IsApproved,
                Specialty = user.Specialty,
                PatientID = user.PatientID,
                CreatedAt = user.CreatedAt
            };

            // Get user documents (if patient)
            if (userRole == "Patient")
            {
                UserDocuments = await _context.PatientDocuments
                    .Where(d => d.PatientId == user.Id)
                    .OrderByDescending(d => d.UploadedAt)
                    .ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDownloadDocumentAsync(int documentId)
        {
            var adminUser = await _userManager.GetUserAsync(User);
            if (adminUser == null)
            {
                return Unauthorized();
            }

            var document = await _context.PatientDocuments
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);

            if (document == null)
            {
                return NotFound();
            }

            // Generate a simple document content if file doesn't exist
            string content;
            string contentType;
            string fileName;

            if (System.IO.File.Exists(document.FilePath))
            {
                var fileBytes = await System.IO.File.ReadAllBytesAsync(document.FilePath);
                contentType = document.MimeType ?? "application/octet-stream";
                fileName = document.Title + System.IO.Path.GetExtension(document.FilePath);
                return File(fileBytes, contentType, fileName);
            }
            else
            {
                // Generate HTML document as fallback
                content = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>{document.Title}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; color: #333; }}
        .header {{ border-bottom: 2px solid #667eea; padding-bottom: 20px; margin-bottom: 30px; }}
        .content {{ line-height: 1.6; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>{document.Title}</h1>
        <p><strong>Document Type:</strong> {document.DocumentType}</p>
        <p><strong>Uploaded:</strong> {document.UploadedAt:MMMM dd, yyyy}</p>
    </div>
    <div class='content'>
        <p><strong>Description:</strong></p>
        <p>{document.Description ?? "No description available."}</p>
    </div>
</body>
</html>";
                contentType = "text/html";
                fileName = document.Title.Replace(" ", "_") + ".html";
                return File(System.Text.Encoding.UTF8.GetBytes(content), contentType, fileName);
            }
        }
    }
}

