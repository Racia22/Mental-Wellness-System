namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for generating PDF documents (appointment confirmations, etc.)
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Generates a PDF confirmation for an appointment
        /// </summary>
        Task<byte[]> GenerateAppointmentPdfAsync(int appointmentId);
    }
}

