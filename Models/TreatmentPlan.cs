using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents a treatment plan assigned by a doctor to a patient
    /// </summary>
    public class TreatmentPlan
    {
        public int TreatmentPlanId { get; set; }

        /// <summary>
        /// Patient User ID
        /// </summary>
        [Required]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        /// Doctor User ID who created the plan
        /// </summary>
        [Required]
        public string DoctorId { get; set; } = string.Empty;

        /// <summary>
        /// Title of the treatment plan
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Description of the treatment plan
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Treatment goals
        /// </summary>
        [MaxLength(1000)]
        public string? Goals { get; set; }

        /// <summary>
        /// Treatment tasks/exercises
        /// </summary>
        [MaxLength(2000)]
        public string? Tasks { get; set; }

        /// <summary>
        /// Start date of the treatment plan
        /// </summary>
        [Required]
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// End date of the treatment plan (optional)
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Status: Active, Completed, Cancelled
        /// </summary>
        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        /// <summary>
        /// Progress notes
        /// </summary>
        [MaxLength(2000)]
        public string? ProgressNotes { get; set; }

        /// <summary>
        /// Related appointment ID (if created from an appointment)
        /// </summary>
        public int? AppointmentId { get; set; }

        /// <summary>
        /// Related patient record ID
        /// </summary>
        public int? PatientRecordId { get; set; }

        /// <summary>
        /// Timestamp when plan was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when plan was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser Patient { get; set; } = null!;
        public virtual ApplicationUser Doctor { get; set; } = null!;
        public virtual Appointment? Appointment { get; set; }
        public virtual PatientRecord? PatientRecord { get; set; }
    }
}

