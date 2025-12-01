using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Pages.Admin
{
    /// <summary>
    /// Reports page - shows system reports and statistics
    /// </summary>
    [Authorize(Policy = "RequireAdminRole")]
    public class ReportsModel : PageModel
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsModel(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Period { get; set; } = "ThisMonth"; // Today, ThisMonth, LastDay, LastWeek, LastMonth, LastYear

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SpecializationFilter { get; set; }

        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public double NoShowRate { get; set; }
        public List<Appointment> RecentAppointments { get; set; } = new();
        public List<AuditLog> RecentAuditLogs { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;

            switch (Period)
            {
                case "Today":
                    startDate = DateTime.UtcNow.Date;
                    endDate = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    endDate = DateTime.UtcNow;
                    break;
                case "LastDay":
                    startDate = DateTime.UtcNow.Date.AddDays(-1);
                    endDate = DateTime.UtcNow.Date.AddTicks(-1);
                    break;
                case "LastWeek":
                    startDate = DateTime.UtcNow.AddDays(-7);
                    endDate = DateTime.UtcNow;
                    break;
                case "LastMonth":
                    startDate = DateTime.UtcNow.AddMonths(-1);
                    endDate = DateTime.UtcNow;
                    break;
                case "LastYear":
                    startDate = DateTime.UtcNow.AddYears(-1);
                    endDate = DateTime.UtcNow;
                    break;
                default:
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    endDate = DateTime.UtcNow;
                    break;
            }

            var appointments = await _context.Appointments
                .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
                .ToListAsync();

            TotalAppointments = appointments.Count;
            CompletedAppointments = appointments.Count(a => a.Status == "Completed");
            CancelledAppointments = appointments.Count(a => a.Status == "Cancelled");
            NoShowAppointments = appointments.Count(a => a.Status == "Scheduled" && a.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow));
            NoShowRate = TotalAppointments > 0 ? (double)NoShowAppointments / TotalAppointments * 100 : 0;

            RecentAppointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToListAsync();

            var auditLogsQuery = _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate);

            // Apply specialization filter if provided - filter to show only doctors with that specialization
            if (!string.IsNullOrEmpty(SpecializationFilter))
            {
                // Get all users in the Doctor role
                var allUsers = await _context.Users.ToListAsync();
                var doctorIdsWithSpecialization = new List<string>();

                foreach (var u in allUsers)
                {
                    var roles = await _userManager.GetRolesAsync(u);
                    if (roles.Contains("Doctor") && 
                        !string.IsNullOrEmpty(u.Specialty))
                    {
                        // Match specialization (case-insensitive, partial match)
                        if (u.Specialty.Contains(SpecializationFilter, StringComparison.OrdinalIgnoreCase) || 
                            u.Specialty.Equals(SpecializationFilter, StringComparison.OrdinalIgnoreCase))
                        {
                            doctorIdsWithSpecialization.Add(u.Id);
                        }
                    }
                }

                if (doctorIdsWithSpecialization.Any())
                {
                    auditLogsQuery = auditLogsQuery.Where(a => 
                        a.User != null && 
                        doctorIdsWithSpecialization.Contains(a.UserId)
                    );
                }
                else
                {
                    // If no doctors found with that specialization, return empty result
                    auditLogsQuery = auditLogsQuery.Where(a => false);
                }
            }

            RecentAuditLogs = await auditLogsQuery
                .OrderByDescending(a => a.Timestamp)
                .Take(50)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostDownloadPdfAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Identity/Account/Login");
            }

            DateTime startDate;
            DateTime endDate = DateTime.UtcNow;

            switch (Period)
            {
                case "Today":
                    startDate = DateTime.UtcNow.Date;
                    endDate = DateTime.UtcNow.Date.AddDays(1).AddTicks(-1);
                    break;
                case "ThisMonth":
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    endDate = DateTime.UtcNow;
                    break;
                case "LastDay":
                    startDate = DateTime.UtcNow.Date.AddDays(-1);
                    endDate = DateTime.UtcNow.Date.AddTicks(-1);
                    break;
                case "LastWeek":
                    startDate = DateTime.UtcNow.AddDays(-7);
                    endDate = DateTime.UtcNow;
                    break;
                case "LastMonth":
                    startDate = DateTime.UtcNow.AddMonths(-1);
                    endDate = DateTime.UtcNow;
                    break;
                case "LastYear":
                    startDate = DateTime.UtcNow.AddYears(-1);
                    endDate = DateTime.UtcNow;
                    break;
                default:
                    startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                    endDate = DateTime.UtcNow;
                    break;
            }

            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.CreatedAt >= startDate && a.CreatedAt <= endDate)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            var auditLogs = await _context.AuditLogs
                .Include(a => a.User)
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            // Generate HTML report content (can be printed to PDF)
            var htmlContent = GenerateReportsPdfHtml(appointments, auditLogs, Period, startDate, endDate);
            return File(System.Text.Encoding.UTF8.GetBytes(htmlContent), "text/html", $"Reports_{Period}_{DateTime.UtcNow:yyyyMMdd}.html");
        }

        private string GenerateReportsPdfHtml(List<Appointment> appointments, List<AuditLog> auditLogs, string period, DateTime startDate, DateTime endDate)
        {
            var totalAppointments = appointments.Count;
            var completed = appointments.Count(a => a.Status == "Completed");
            var cancelled = appointments.Count(a => a.Status == "Cancelled");
            var noShow = appointments.Count(a => a.Status == "Scheduled" && a.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow));
            var noShowRate = totalAppointments > 0 ? (double)noShow / totalAppointments * 100 : 0;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>System Reports - {period}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; color: #333; }}
        .header {{ text-align: center; border-bottom: 3px solid #007bff; padding-bottom: 20px; margin-bottom: 30px; }}
        .logo {{ font-size: 28px; font-weight: bold; color: #007bff; margin-bottom: 10px; }}
        .stats {{ display: flex; justify-content: space-around; margin: 30px 0; }}
        .stat-box {{ border: 1px solid #ddd; padding: 15px; text-align: center; flex: 1; margin: 0 10px; }}
        .stat-value {{ font-size: 24px; font-weight: bold; color: #007bff; }}
        .stat-label {{ color: #666; margin-top: 5px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        th {{ background-color: #007bff; color: white; }}
        .footer {{ margin-top: 40px; padding-top: 20px; border-top: 1px solid #ddd; text-align: center; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='logo'>Mental Wellness System</div>
        <div>System Reports - {period}</div>
        <div style='font-size: 14px; color: #666; margin-top: 10px;'>
            Period: {startDate:MMMM dd, yyyy} to {endDate:MMMM dd, yyyy}
        </div>
    </div>
    
    <div class='stats'>
        <div class='stat-box'>
            <div class='stat-value'>{totalAppointments}</div>
            <div class='stat-label'>Total Appointments</div>
        </div>
        <div class='stat-box'>
            <div class='stat-value'>{completed}</div>
            <div class='stat-label'>Completed</div>
        </div>
        <div class='stat-box'>
            <div class='stat-value'>{cancelled}</div>
            <div class='stat-label'>Cancelled</div>
        </div>
        <div class='stat-box'>
            <div class='stat-value'>{noShowRate:F1}%</div>
            <div class='stat-label'>No-Show Rate</div>
        </div>
    </div>

    <h3>Recent Appointments</h3>
    <table>
        <thead>
            <tr>
                <th>Date</th>
                <th>Patient</th>
                <th>Doctor</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>
            {(appointments.Any() ? string.Join("", appointments.Take(20).Select(a => $@"
            <tr>
                <td>{a.AppointmentDate:MMMM dd, yyyy}</td>
                <td>{a.Patient.FullName}</td>
                <td>{a.Doctor.FullName}</td>
                <td>{a.Status}</td>
            </tr>")) : "<tr><td colspan='4'>No appointments found</td></tr>")}
        </tbody>
    </table>

    <h3>Recent Activity Log</h3>
    <table>
        <thead>
            <tr>
                <th>Timestamp</th>
                <th>User</th>
                <th>Action</th>
                <th>Entity</th>
                <th>Description</th>
            </tr>
        </thead>
        <tbody>
            {(auditLogs.Any() ? string.Join("", auditLogs.Take(20).Select(log => $@"
            <tr>
                <td>{log.Timestamp:MMM dd, yyyy HH:mm}</td>
                <td>{log.User?.FullName ?? "Unknown"}</td>
                <td>{log.Action}</td>
                <td>{log.EntityType}</td>
                <td>{log.Description}</td>
            </tr>")) : "<tr><td colspan='5'>No activity logs found</td></tr>")}
        </tbody>
    </table>
    
    <div class='footer'>
        <p>Generated on {DateTime.UtcNow:MMMM dd, yyyy 'at' hh:mm tt} UTC</p>
        <p>&copy; {DateTime.UtcNow.Year} Mental Wellness System. All rights reserved.</p>
    </div>
</body>
</html>";
        }
    }
}

