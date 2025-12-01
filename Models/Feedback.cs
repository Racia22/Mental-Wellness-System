using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents patient feedback/rating for an appointment or doctor
    /// </summary>
    public class Feedback
    {
        public int FeedbackId { get; set; }

        /// <summary>
        /// Patient User ID who provided feedback
        /// </summary>
        [Required]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        /// Doctor User ID being rated
        /// </summary>
        [Required]
        public string DoctorId { get; set; } = string.Empty;

        /// <summary>
        /// Related appointment ID
        /// </summary>
        [Required]
        public int AppointmentId { get; set; }

        /// <summary>
        /// Rating (1-5 stars)
        /// </summary>
        [Range(1, 5)]
        public int Rating { get; set; }

        /// <summary>
        /// Feedback comments
        /// </summary>
        [MaxLength(1000)]
        public string? Comments { get; set; }

        /// <summary>
        /// Whether feedback is anonymous
        /// </summary>
        public bool IsAnonymous { get; set; } = false;

        /// <summary>
        /// Timestamp when feedback was submitted
        /// </summary>
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser Patient { get; set; } = null!;
        public virtual ApplicationUser Doctor { get; set; } = null!;
        public virtual Appointment Appointment { get; set; } = null!;
    }
}

