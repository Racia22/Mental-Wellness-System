using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for automated reporting and analytics
    /// </summary>
    public interface IReportingService
    {
        /// <summary>
        /// Generate weekly report for a doctor
        /// </summary>
        Task<string> GenerateWeeklyDoctorReportAsync(string doctorId);

        /// <summary>
        /// Generate monthly admin report
        /// </summary>
        Task<string> GenerateMonthlyAdminReportAsync();

        /// <summary>
        /// Get dashboard statistics for admin
        /// </summary>
        Task<AdminDashboardStats> GetAdminDashboardStatsAsync();

        /// <summary>
        /// Get dashboard statistics for doctor
        /// </summary>
        Task<DoctorDashboardStats> GetDoctorDashboardStatsAsync(string doctorId);

        /// <summary>
        /// Get dashboard statistics for patient
        /// </summary>
        Task<PatientDashboardStats> GetPatientDashboardStatsAsync(string patientId);
    }

    public class AdminDashboardStats
    {
        public int TotalUsers { get; set; }
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int PendingDoctorApprovals { get; set; }
        public int TotalAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int TotalPatientRecords { get; set; }
        public int ActiveTreatmentPlans { get; set; }
        public int TotalMoodLogs { get; set; }
    }

    public class DoctorDashboardStats
    {
        public int TotalAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int TodayAppointments { get; set; }
        public int TotalPatients { get; set; }
        public int TotalPatientRecords { get; set; }
        public int ActiveTreatmentPlans { get; set; }
        public double AverageRating { get; set; }
        public int TotalFeedbacks { get; set; }
    }

    public class PatientDashboardStats
    {
        public int UpcomingAppointments { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int TotalMoodLogs { get; set; }
        public int UnviewedRecommendations { get; set; }
        public int ActiveTreatmentPlans { get; set; }
        public double? AverageMoodScore { get; set; }
    }
}

