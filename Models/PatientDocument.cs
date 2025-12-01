using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents a document uploaded for a patient (prescriptions, reports, etc.)
    /// </summary>
    public class PatientDocument
    {
        public int DocumentId { get; set; }

        /// <summary>
        /// Patient User ID
        /// </summary>
        [Required]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        /// Doctor User ID who uploaded the document (if applicable)
        /// </summary>
        public string? DoctorId { get; set; }

        /// <summary>
        /// Document title/name
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Document type: Prescription, Report, TestResult, Other
        /// </summary>
        [MaxLength(50)]
        public string DocumentType { get; set; } = "Other";

        /// <summary>
        /// File path or URL
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long? FileSize { get; set; }

        /// <summary>
        /// MIME type
        /// </summary>
        [MaxLength(100)]
        public string? MimeType { get; set; }

        /// <summary>
        /// Description of the document
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Related appointment ID (if applicable)
        /// </summary>
        public int? AppointmentId { get; set; }

        /// <summary>
        /// Related patient record ID (if applicable)
        /// </summary>
        public int? PatientRecordId { get; set; }

        /// <summary>
        /// Timestamp when document was uploaded
        /// </summary>
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser Patient { get; set; } = null!;
        public virtual ApplicationUser? Doctor { get; set; }
        public virtual Appointment? Appointment { get; set; }
        public virtual PatientRecord? PatientRecord { get; set; }
    }
}

