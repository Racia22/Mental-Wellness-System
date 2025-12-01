using Microsoft.AspNetCore.Identity;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Custom Identity User that extends IdentityUser with Mental Wellness System specific properties
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Full name of the user
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Unique Patient ID (format: MW-YYYYMMDD-XXXX-C) - only for patients
        /// </summary>
        public string? PatientID { get; set; }

        /// <summary>
        /// Phone number (stored in base IdentityUser.PhoneNumber, but we keep this for compatibility)
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// For doctors: approval status (pending admin approval)
        /// </summary>
        public bool IsApproved { get; set; } = false;

        /// <summary>
        /// For doctors: specialty/area of expertise
        /// </summary>
        public string? Specialty { get; set; }

        /// <summary>
        /// For patients: age
        /// </summary>
        public int? Age { get; set; }

        /// <summary>
        /// For patients: gender
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// For patients: category/therapy type
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Timestamp when user was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

