using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for managing appointments
    /// </summary>
    public interface IAppointmentService
    {
        /// <summary>
        /// Books a new appointment with double-booking prevention
        /// </summary>
        Task<Appointment> BookAppointmentAsync(string patientId, string doctorId, DateOnly date, TimeOnly time, string? appointmentType);

        /// <summary>
        /// Checks if a time slot is available for a doctor
        /// </summary>
        Task<bool> IsTimeSlotAvailableAsync(string doctorId, DateOnly date, TimeOnly time);

        /// <summary>
        /// Sends reminders for upcoming appointments
        /// </summary>
        Task SendUpcomingAppointmentRemindersAsync();
    }
}

