namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Audit log for tracking access to sensitive data (e.g., patient records)
    /// </summary>
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        /// <summary>
        /// User ID who performed the action
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Action performed: View, Create, Update, Delete
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Entity type: PatientRecord, Appointment, etc.
        /// </summary>
        public string EntityType { get; set; } = string.Empty;

        /// <summary>
        /// ID of the affected entity
        /// </summary>
        public int? EntityId { get; set; }

        /// <summary>
        /// Description of the action
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// IP address of the user
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// Timestamp when action was performed
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        public virtual ApplicationUser? User { get; set; }
    }
}

