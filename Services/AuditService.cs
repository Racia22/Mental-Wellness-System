using MentalWellnessSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Implementation of audit service
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly MentalWellnessDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            MentalWellnessDbContext context,
            ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogPatientRecordAccessAsync(string userId, int recordId, string action, string? ipAddress = null)
        {
            await LogEntityAccessAsync(userId, "PatientRecord", recordId, action, 
                $"Accessed patient record {recordId}", ipAddress);
        }

        public async Task LogEntityAccessAsync(string userId, string entityType, int? entityId, string action, string description, string? ipAddress = null)
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                EntityType = entityType,
                EntityId = entityId,
                Action = action,
                Description = description,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Audit log created: {Action} on {EntityType} {EntityId} by {UserId}", 
                action, entityType, entityId, userId);
        }
    }
}

