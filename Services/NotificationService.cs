using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Implementation of notification service
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly MentalWellnessDbContext _context;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            MentalWellnessDbContext context,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SendBookingConfirmationAsync(Appointment appointment)
        {
            var patient = await _userManager.FindByIdAsync(appointment.PatientId);
            var doctor = await _userManager.FindByIdAsync(appointment.DoctorId);

            if (patient == null || doctor == null)
            {
                _logger.LogWarning("Cannot send booking confirmation: Patient or Doctor not found");
                return;
            }

            var subject = "Appointment Booking Confirmation";
            var message = $@"
Dear {patient.FullName},

Your appointment has been successfully booked.

Appointment Details:
- Doctor: {doctor.FullName} ({doctor.Specialty})
- Date: {appointment.AppointmentDate:MMMM dd, yyyy}
- Time: {appointment.AppointmentTime:hh:mm tt}
- Type: {appointment.AppointmentType ?? "General Consultation"}

Please arrive 10 minutes before your scheduled time.

Thank you,
Mental Wellness System
";

            await SendNotificationAsync(
                appointment.PatientId,
                "Booking",
                subject,
                message,
                "Email",
                appointment.AppointmentId);
        }

        public async Task SendAppointmentReminderAsync(Appointment appointment, int hoursBefore)
        {
            var patient = await _userManager.FindByIdAsync(appointment.PatientId);
            var doctor = await _userManager.FindByIdAsync(appointment.DoctorId);

            if (patient == null || doctor == null)
            {
                return;
            }

            var subject = $"Appointment Reminder - {hoursBefore} hour(s) before";
            var message = $@"
Dear {patient.FullName},

This is a reminder that you have an appointment in {hoursBefore} hour(s).

Appointment Details:
- Doctor: {doctor.FullName}
- Date: {appointment.AppointmentDate:MMMM dd, yyyy}
- Time: {appointment.AppointmentTime:hh:mm tt}

Please be on time.

Thank you,
Mental Wellness System
";

            await SendNotificationAsync(
                appointment.PatientId,
                "Reminder",
                subject,
                message,
                "Email",
                appointment.AppointmentId);
        }

        public async Task SendCancellationNotificationAsync(Appointment appointment, string cancelledBy)
        {
            var patient = await _userManager.FindByIdAsync(appointment.PatientId);
            var doctor = await _userManager.FindByIdAsync(appointment.DoctorId);

            if (patient == null || doctor == null)
            {
                return;
            }

            var recipientId = cancelledBy == "Patient" ? appointment.DoctorId : appointment.PatientId;
            var recipient = await _userManager.FindByIdAsync(recipientId);

            var subject = "Appointment Cancelled";
            var message = $@"
Dear {recipient?.FullName},

Your appointment has been cancelled by {cancelledBy}.

Appointment Details:
- Doctor: {doctor.FullName}
- Patient: {patient.FullName}
- Date: {appointment.AppointmentDate:MMMM dd, yyyy}
- Time: {appointment.AppointmentTime:hh:mm tt}

If you need to reschedule, please contact us.

Thank you,
Mental Wellness System
";

            await SendNotificationAsync(
                recipientId,
                "Cancellation",
                subject,
                message,
                "Email",
                appointment.AppointmentId);
        }

        public async Task SendNotificationAsync(string userId, string type, string subject, string message, string deliveryMethod = "Email", int? appointmentId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                NotificationType = type,
                Subject = subject,
                Message = message,
                DeliveryMethod = deliveryMethod,
                Status = "Pending",
                AppointmentId = appointmentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Notification queued: {Type} for user {UserId}", type, userId);
        }

        public async Task ProcessPendingNotificationsAsync()
        {
            var pendingNotifications = await _context.Notifications
                .Where(n => n.Status == "Pending")
                .OrderBy(n => n.CreatedAt)
                .Take(50) // Process in batches
                .ToListAsync();

            foreach (var notification in pendingNotifications)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(notification.UserId);
                    if (user == null)
                    {
                        notification.Status = "Failed";
                        notification.ErrorMessage = "User not found";
                        notification.SentAt = DateTime.UtcNow;
                        continue;
                    }

                    bool success = false;

                    if (notification.DeliveryMethod == "Email" || notification.DeliveryMethod == "Both")
                    {
                        success = await _emailService.SendEmailAsync(
                            user.Email!,
                            notification.Subject,
                            notification.Message);
                    }

                    if (success)
                    {
                        notification.Status = "Sent";
                        notification.SentAt = DateTime.UtcNow;
                        _logger.LogInformation("Notification sent: {NotificationId}", notification.NotificationId);
                    }
                    else
                    {
                        notification.Status = "Failed";
                        notification.ErrorMessage = "Email sending failed";
                        notification.SentAt = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    notification.Status = "Failed";
                    notification.ErrorMessage = ex.Message;
                    notification.SentAt = DateTime.UtcNow;
                    _logger.LogError(ex, "Error processing notification {NotificationId}", notification.NotificationId);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

