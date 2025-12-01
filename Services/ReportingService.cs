using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Implementation of reporting and analytics service
    /// </summary>
    public class ReportingService : IReportingService
    {
        private readonly MentalWellnessDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ReportingService> _logger;

        public ReportingService(
            MentalWellnessDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ReportingService> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GenerateWeeklyDoctorReportAsync(string doctorId)
        {
            var startDate = DateTime.UtcNow.AddDays(-7);
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.CreatedAt >= startDate)
                .CountAsync();

            var completedRecords = await _context.PatientRecords
                .Where(r => r.DoctorId == doctorId && r.CreatedAt >= startDate)
                .CountAsync();

            return $"Weekly Report for Doctor:\n" +
                   $"Appointments: {appointments}\n" +
                   $"Patient Records Created: {completedRecords}\n" +
                   $"Report Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
        }

        public async Task<string> GenerateMonthlyAdminReportAsync()
        {
            var startDate = DateTime.UtcNow.AddMonths(-1);
            var totalUsers = await _context.Users.CountAsync();
            var newUsers = await _context.Users
                .Where(u => u.CreatedAt >= startDate)
                .CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();
            var newAppointments = await _context.Appointments
                .Where(a => a.CreatedAt >= startDate)
                .CountAsync();

            return $"Monthly Admin Report:\n" +
                   $"Total Users: {totalUsers}\n" +
                   $"New Users (Last Month): {newUsers}\n" +
                   $"Total Appointments: {totalAppointments}\n" +
                   $"New Appointments (Last Month): {newAppointments}\n" +
                   $"Report Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
        }

        public async Task<AdminDashboardStats> GetAdminDashboardStatsAsync()
        {
            var users = await _context.Users.ToListAsync();
            var patients = users.Where(u => _userManager.IsInRoleAsync(u, "Patient").Result).ToList();
            var doctors = users.Where(u => _userManager.IsInRoleAsync(u, "Doctor").Result).ToList();

            return new AdminDashboardStats
            {
                TotalUsers = users.Count,
                TotalPatients = patients.Count,
                TotalDoctors = doctors.Count,
                PendingDoctorApprovals = doctors.Count(d => !d.IsApproved),
                TotalAppointments = await _context.Appointments.CountAsync(),
                UpcomingAppointments = await _context.Appointments
                    .Where(a => a.AppointmentDate >= DateOnly.FromDateTime(DateTime.UtcNow))
                    .CountAsync(),
                CompletedAppointments = await _context.Appointments
                    .Where(a => a.Status == "Completed")
                    .CountAsync(),
                TotalPatientRecords = await _context.PatientRecords.CountAsync(),
                ActiveTreatmentPlans = await _context.TreatmentPlans
                    .Where(t => t.Status == "Active")
                    .CountAsync(),
                TotalMoodLogs = await _context.MoodLogs.CountAsync()
            };
        }

        public async Task<DoctorDashboardStats> GetDoctorDashboardStatsAsync(string doctorId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            var feedbacks = await _context.Feedbacks
                .Where(f => f.DoctorId == doctorId)
                .ToListAsync();

            return new DoctorDashboardStats
            {
                TotalAppointments = appointments.Count,
                UpcomingAppointments = appointments
                    .Count(a => a.AppointmentDate >= today),
                TodayAppointments = appointments
                    .Count(a => a.AppointmentDate == today),
                TotalPatients = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId)
                    .Select(a => a.PatientId)
                    .Distinct()
                    .CountAsync(),
                TotalPatientRecords = await _context.PatientRecords
                    .Where(r => r.DoctorId == doctorId)
                    .CountAsync(),
                ActiveTreatmentPlans = await _context.TreatmentPlans
                    .Where(t => t.DoctorId == doctorId && t.Status == "Active")
                    .CountAsync(),
                AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0,
                TotalFeedbacks = feedbacks.Count
            };
        }

        public async Task<PatientDashboardStats> GetPatientDashboardStatsAsync(string patientId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .ToListAsync();

            var moodLogs = await _context.MoodLogs
                .Where(m => m.PatientId == patientId)
                .ToListAsync();

            return new PatientDashboardStats
            {
                UpcomingAppointments = appointments
                    .Count(a => a.AppointmentDate >= today),
                TotalAppointments = appointments.Count,
                CompletedAppointments = appointments
                    .Count(a => a.Status == "Completed"),
                TotalMoodLogs = moodLogs.Count,
                UnviewedRecommendations = await _context.ResourceRecommendations
                    .Where(r => r.PatientId == patientId && !r.IsViewed)
                    .CountAsync(),
                ActiveTreatmentPlans = await _context.TreatmentPlans
                    .Where(t => t.PatientId == patientId && t.Status == "Active")
                    .CountAsync(),
                AverageMoodScore = moodLogs.Any() ? moodLogs.Average(m => m.MoodScore) : null
            };
        }
    }
}

