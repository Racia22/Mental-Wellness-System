using System.Net;
using System.Net.Mail;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Email service implementation using SMTP
    /// Configure SMTP settings in appsettings.json
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Get SMTP settings from configuration
                var smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
                var smtpUser = _configuration["EmailSettings:SmtpUser"];
                var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
                var fromEmail = _configuration["EmailSettings:FromEmail"] ?? smtpUser;
                var fromName = _configuration["EmailSettings:FromName"] ?? "Mental Wellness System";

                // If SMTP is not configured, log and return false (for development)
                if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("Email not sent - SMTP not configured. To: {To}, Subject: {Subject}", to, subject);
                    _logger.LogInformation("Email body would be: {Body}", body);
                    return false; // Return false in production, true in development for testing
                }

                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUser, smtpPassword)
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(fromEmail ?? smtpUser ?? "noreply@mentalwellness.com", fromName ?? "Mental Wellness System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };

                message.To.Add(to);

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {To}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To}", to);
                return false;
            }
        }
    }
}

