using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for sending notifications (email/SMS) to users
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Sends an appointment booking confirmation notification
        /// </summary>
        Task SendBookingConfirmationAsync(Appointment appointment);

        /// <summary>
        /// Sends appointment reminder notifications (24h and 1h before)
        /// </summary>
        Task SendAppointmentReminderAsync(Appointment appointment, int hoursBefore);

        /// <summary>
        /// Sends appointment cancellation notification
        /// </summary>
        Task SendCancellationNotificationAsync(Appointment appointment, string cancelledBy);

        /// <summary>
        /// Sends a custom notification to a user
        /// </summary>
        Task SendNotificationAsync(string userId, string type, string subject, string message, string deliveryMethod = "Email", int? appointmentId = null);

        /// <summary>
        /// Processes pending notifications (called by background service)
        /// </summary>
        Task ProcessPendingNotificationsAsync();
    }
}

