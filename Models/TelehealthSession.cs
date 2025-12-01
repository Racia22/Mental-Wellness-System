using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents a telehealth/virtual consultation session
    /// </summary>
    public class TelehealthSession
    {
        public int TelehealthSessionId { get; set; }

        /// <summary>
        /// Related appointment ID
        /// </summary>
        [Required]
        public int AppointmentId { get; set; }

        /// <summary>
        /// Session URL (video call link, chat room, etc.)
        /// </summary>
        [MaxLength(500)]
        public string? SessionUrl { get; set; }

        /// <summary>
        /// Session type: Video, Audio, Chat
        /// </summary>
        [MaxLength(20)]
        public string SessionType { get; set; } = "Video";

        /// <summary>
        /// Session start time
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Session end time
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Session duration in minutes
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// Session status: Scheduled, InProgress, Completed, Cancelled
        /// </summary>
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        /// <summary>
        /// Session notes
        /// </summary>
        [MaxLength(2000)]
        public string? SessionNotes { get; set; }

        /// <summary>
        /// Recording URL (if session was recorded)
        /// </summary>
        [MaxLength(500)]
        public string? RecordingUrl { get; set; }

        /// <summary>
        /// Timestamp when session was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when session was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual Appointment Appointment { get; set; } = null!;
    }
}

