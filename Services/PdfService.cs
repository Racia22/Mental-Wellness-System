using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// PDF service implementation using iTextSharp or similar library
    /// For this implementation, we'll use a simple HTML-to-PDF approach
    /// In production, consider using QuestPDF, iTextSharp, or similar library
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<PdfService> _logger;

        public PdfService(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<PdfService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<byte[]> GenerateAppointmentPdfAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
            {
                throw new ArgumentException("Appointment not found", nameof(appointmentId));
            }

            // Generate HTML content for PDF
            var htmlContent = GenerateAppointmentHtml(appointment);

            // Return HTML content that can be opened as PDF by browser
            // The browser will handle the PDF conversion when user prints/saves
            return System.Text.Encoding.UTF8.GetBytes(htmlContent);
        }

        private string GenerateAppointmentHtml(Appointment appointment)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Appointment Confirmation - {appointment.AppointmentId}</title>
    <style>
        @media print {{
            @page {{
                margin: 1cm;
            }}
            body {{
                margin: 0;
            }}
        }}
        body {{
            font-family: Arial, sans-serif;
            margin: 40px;
            color: #333;
            line-height: 1.6;
        }}
        .header {{
            text-align: center;
            border-bottom: 3px solid #007bff;
            padding-bottom: 20px;
            margin-bottom: 30px;
        }}
        .logo {{
            font-size: 28px;
            font-weight: bold;
            color: #007bff;
            margin-bottom: 10px;
        }}
        .content {{
            margin: 30px 0;
        }}
        .section {{
            margin-bottom: 20px;
            padding: 10px 0;
            border-bottom: 1px solid #eee;
        }}
        .label {{
            font-weight: bold;
            color: #555;
            display: inline-block;
            width: 180px;
            vertical-align: top;
        }}
        .value {{
            color: #333;
            display: inline-block;
            width: calc(100% - 200px);
        }}
        .footer {{
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #ddd;
            text-align: center;
            color: #666;
            font-size: 12px;
        }}
        .patient-id {{
            background-color: #f0f0f0;
            padding: 10px;
            border-radius: 5px;
            font-family: monospace;
            font-size: 16px;
            margin: 10px 0;
            display: inline-block;
        }}
        .appointment-details {{
            background-color: #f9f9f9;
            padding: 20px;
            border-radius: 5px;
            margin: 20px 0;
        }}
    </style>
    <script>
        window.onload = function() {{
            // Auto-print when opened (optional - can be removed if not desired)
            // window.print();
        }}
    </script>
</head>
<body>
    <div class='header'>
        <div class='logo'>Mental Wellness System</div>
        <div style='font-size: 18px; margin-top: 10px;'>Appointment Confirmation</div>
    </div>
    
    <div class='content'>
        <div class='appointment-details'>
            <div class='section'>
                <span class='label'>Appointment ID:</span>
                <span class='value'><strong>#{appointment.AppointmentId}</strong></span>
            </div>
            
            <div class='section'>
                <span class='label'>Patient Name:</span>
                <span class='value'>{appointment.Patient.FullName}</span>
            </div>
            
            <div class='section'>
                <span class='label'>Patient ID:</span>
                <span class='value'>
                    <div class='patient-id'>{appointment.Patient.PatientID ?? "N/A"}</div>
                </span>
            </div>
            
            <div class='section'>
                <span class='label'>Doctor Name:</span>
                <span class='value'>{appointment.Doctor.FullName}</span>
            </div>
            
            <div class='section'>
                <span class='label'>Specialty:</span>
                <span class='value'>{appointment.Doctor.Specialty ?? "General"}</span>
            </div>
            
            <div class='section'>
                <span class='label'>Appointment Date:</span>
                <span class='value'><strong>{appointment.AppointmentDate:MMMM dd, yyyy}</strong></span>
            </div>
            
            <div class='section'>
                <span class='label'>Appointment Time:</span>
                <span class='value'><strong>{appointment.AppointmentTime:hh:mm tt}</strong></span>
            </div>
            
            <div class='section'>
                <span class='label'>Appointment Type:</span>
                <span class='value'>{appointment.AppointmentType ?? "General Consultation"}</span>
            </div>
            
            <div class='section'>
                <span class='label'>Status:</span>
                <span class='value'><strong>{appointment.Status}</strong></span>
            </div>
            
            @if (!string.IsNullOrEmpty(appointment.Notes))
            {{
                <div class='section'>
                    <span class='label'>Notes:</span>
                    <span class='value'>{appointment.Notes}</span>
                </div>
            }}
        </div>
    </div>
    
    <div class='footer'>
        <p><strong>This is an official confirmation document.</strong></p>
        <p>Please bring this document with you to your appointment.</p>
        <p>Generated on {DateTime.UtcNow:MMMM dd, yyyy 'at' hh:mm tt} UTC</p>
        <p>&copy; {DateTime.UtcNow.Year} Mental Wellness System. All rights reserved.</p>
    </div>
</body>
</html>";
        }
    }
}

