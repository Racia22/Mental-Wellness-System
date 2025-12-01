namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents a notification sent to users (email/SMS)
    /// </summary>
    public class Notification
    {
        public int NotificationId { get; set; }

        /// <summary>
        /// User ID (from Identity) who receives the notification
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Type of notification: Booking, Reminder, Cancellation, etc.
        /// </summary>
        public string NotificationType { get; set; } = string.Empty;

        /// <summary>
        /// Subject/title of the notification
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Message content
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Delivery method: Email, SMS, or Both
        /// </summary>
        public string DeliveryMethod { get; set; } = "Email";

        /// <summary>
        /// Status: Pending, Sent, Failed
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Timestamp when notification was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when notification was sent
        /// </summary>
        public DateTime? SentAt { get; set; }

        /// <summary>
        /// Error message if sending failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Related appointment ID (if applicable)
        /// </summary>
        public int? AppointmentId { get; set; }

        // Navigation properties
        public virtual ApplicationUser? User { get; set; }
        public virtual Appointment? Appointment { get; set; }
    }
}

