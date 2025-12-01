using MentalWellnessSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Implementation of appointment service with transactional booking
    /// </summary>
    public class AppointmentService : IAppointmentService
    {
        private readonly MentalWellnessDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            MentalWellnessDbContext context,
            INotificationService notificationService,
            ILogger<AppointmentService> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<Appointment> BookAppointmentAsync(string patientId, string doctorId, DateOnly date, TimeOnly time, string? appointmentType)
        {
            // Use database transaction to prevent double-booking
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if time slot is available (with lock to prevent race conditions)
                var isAvailable = await IsTimeSlotAvailableAsync(doctorId, date, time);
                if (!isAvailable)
                {
                    throw new InvalidOperationException("The selected time slot is no longer available. Please choose another time.");
                }

                // Create appointment
                var appointment = new Appointment
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    AppointmentDate = date,
                    AppointmentTime = time,
                    AppointmentType = appointmentType ?? "General Consultation",
                    Status = "Scheduled",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                // Reload appointment with navigation properties
                appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .FirstAsync(a => a.AppointmentId == appointment.AppointmentId);

                // Send booking confirmation notification (async, don't wait)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.SendBookingConfirmationAsync(appointment);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending booking confirmation for appointment {AppointmentId}", appointment.AppointmentId);
                    }
                });

                _logger.LogInformation("Appointment booked: {AppointmentId} for patient {PatientId} with doctor {DoctorId}", 
                    appointment.AppointmentId, patientId, doctorId);

                return appointment;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> IsTimeSlotAvailableAsync(string doctorId, DateOnly date, TimeOnly time)
        {
            // Check if doctor has an existing appointment at this time
            var existingAppointment = await _context.Appointments
                .Where(a => a.DoctorId == doctorId
                         && a.AppointmentDate == date
                         && a.AppointmentTime == time
                         && a.Status != "Cancelled")
                .FirstOrDefaultAsync();

            return existingAppointment == null;
        }

        public async Task SendUpcomingAppointmentRemindersAsync()
        {
            var now = DateTime.UtcNow;
            var tomorrow = now.AddHours(24);
            var oneHourLater = now.AddHours(1);

            // Get appointments that are 24 hours away (within a 5-minute window)
            var appointments24h = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Status == "Scheduled"
                         && a.AppointmentDate == DateOnly.FromDateTime(tomorrow)
                         && a.AppointmentTime.Hour == tomorrow.Hour
                         && a.AppointmentTime.Minute >= tomorrow.Minute - 5
                         && a.AppointmentTime.Minute <= tomorrow.Minute + 5)
                .ToListAsync();

            foreach (var appointment in appointments24h)
            {
                // Check if we already sent a 24h reminder
                var hasReminder = await _context.Notifications
                    .AnyAsync(n => n.AppointmentId == appointment.AppointmentId
                                && n.NotificationType == "Reminder"
                                && n.CreatedAt >= now.AddHours(-1));

                if (!hasReminder)
                {
                    await _notificationService.SendAppointmentReminderAsync(appointment, 24);
                }
            }

            // Get appointments that are 1 hour away (within a 5-minute window)
            var appointments1h = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Status == "Scheduled"
                         && a.AppointmentDate == DateOnly.FromDateTime(oneHourLater)
                         && a.AppointmentTime.Hour == oneHourLater.Hour
                         && a.AppointmentTime.Minute >= oneHourLater.Minute - 5
                         && a.AppointmentTime.Minute <= oneHourLater.Minute + 5)
                .ToListAsync();

            foreach (var appointment in appointments1h)
            {
                // Check if we already sent a 1h reminder
                var hasReminder = await _context.Notifications
                    .AnyAsync(n => n.AppointmentId == appointment.AppointmentId
                                && n.NotificationType == "Reminder"
                                && n.CreatedAt >= now.AddMinutes(-10));

                if (!hasReminder)
                {
                    await _notificationService.SendAppointmentReminderAsync(appointment, 1);
                }
            }
        }
    }
}

