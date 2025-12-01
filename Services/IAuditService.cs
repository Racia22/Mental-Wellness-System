namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for logging audit trails (access to sensitive data)
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Logs access to a patient record
        /// </summary>
        Task LogPatientRecordAccessAsync(string userId, int recordId, string action, string? ipAddress = null);

        /// <summary>
        /// Logs any entity access
        /// </summary>
        Task LogEntityAccessAsync(string userId, string entityType, int? entityId, string action, string description, string? ipAddress = null);
    }
}

