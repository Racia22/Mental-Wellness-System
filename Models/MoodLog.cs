using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents a daily mood/mental health log entry by a patient
    /// </summary>
    public class MoodLog
    {
        public int MoodLogId { get; set; }

        /// <summary>
        /// Patient User ID (from Identity)
        /// </summary>
        [Required]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        /// Date of the mood log entry
        /// </summary>
        [Required]
        public DateOnly LogDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

        /// <summary>
        /// Mood score (1-10, where 1 is very low, 10 is excellent)
        /// </summary>
        [Range(1, 10)]
        public int MoodScore { get; set; }

        /// <summary>
        /// Stress level (1-10)
        /// </summary>
        [Range(1, 10)]
        public int? StressLevel { get; set; }

        /// <summary>
        /// Sleep hours
        /// </summary>
        [Range(0, 24)]
        public decimal? SleepHours { get; set; }

        /// <summary>
        /// Energy level (1-10)
        /// </summary>
        [Range(1, 10)]
        public int? EnergyLevel { get; set; }

        /// <summary>
        /// Notes about the day
        /// </summary>
        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Timestamp when log was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser Patient { get; set; } = null!;
    }
}

