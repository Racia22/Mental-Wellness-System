namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for sending emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email to the specified recipient
        /// </summary>
        Task<bool> SendEmailAsync(string to, string subject, string body);
    }
}

