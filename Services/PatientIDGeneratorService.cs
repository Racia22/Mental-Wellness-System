using MentalWellnessSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for generating unique Patient IDs in the format: MW-YYYYMMDD-XXXX-C
    /// Where:
    /// - MW: Mental Wellness prefix
    /// - YYYYMMDD: Registration date
    /// - XXXX: 4-digit sequential number
    /// - C: Check digit (0-9)
    /// </summary>
    public interface IPatientIDGeneratorService
    {
        /// <summary>
        /// Generates a unique Patient ID for a new patient registration
        /// </summary>
        Task<string> GeneratePatientIDAsync();
    }

    public class PatientIDGeneratorService : IPatientIDGeneratorService
    {
        private readonly MentalWellnessDbContext _context;
        private readonly ILogger<PatientIDGeneratorService> _logger;

        public PatientIDGeneratorService(
            MentalWellnessDbContext context,
            ILogger<PatientIDGeneratorService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GeneratePatientIDAsync()
        {
            var today = DateTime.UtcNow;
            var datePrefix = today.ToString("yyyyMMdd");
            var basePrefix = $"MW-{datePrefix}-";

            // Find the highest sequential number for today
            var existingIDs = await _context.Users
                .Where(u => u.PatientID != null && u.PatientID.StartsWith(basePrefix))
                .Select(u => u.PatientID)
                .ToListAsync();

            int sequenceNumber = 1;
            if (existingIDs.Any())
            {
                var numbers = existingIDs
                    .Where(id => id != null && id.Length >= basePrefix.Length + 4)
                    .Select(id => id!.Substring(basePrefix.Length, 4))
                    .Where(s => int.TryParse(s, out _))
                    .Select(int.Parse)
                    .ToList();

                if (numbers.Any())
                {
                    sequenceNumber = numbers.Max() + 1;
                }
            }

            // Ensure sequence number is 4 digits (0001-9999)
            if (sequenceNumber > 9999)
            {
                // If we exceed 9999 in one day, append a letter suffix
                _logger.LogWarning("Daily patient registration limit reached. Using extended format.");
                sequenceNumber = 1; // Reset and use extended format if needed
            }

            var sequencePart = sequenceNumber.ToString("D4"); // 4-digit format: 0001, 0002, etc.

            // Generate check digit (simple modulo 10)
            var checkDigit = CalculateCheckDigit($"{basePrefix}{sequencePart}");

            var patientID = $"{basePrefix}{sequencePart}-{checkDigit}";

            // Verify uniqueness (double-check)
            var exists = await _context.Users.AnyAsync(u => u.PatientID == patientID);
            if (exists)
            {
                // Retry with incremented sequence
                sequenceNumber++;
                sequencePart = sequenceNumber.ToString("D4");
                checkDigit = CalculateCheckDigit($"{basePrefix}{sequencePart}");
                patientID = $"{basePrefix}{sequencePart}-{checkDigit}";
            }

            _logger.LogInformation("Generated Patient ID: {PatientID}", patientID);
            return patientID;
        }

        /// <summary>
        /// Calculates a simple check digit for the Patient ID
        /// </summary>
        private int CalculateCheckDigit(string baseID)
        {
            // Simple checksum: sum of all numeric characters modulo 10
            int sum = 0;
            foreach (char c in baseID)
            {
                if (char.IsDigit(c))
                {
                    sum += int.Parse(c.ToString());
                }
            }
            return sum % 10;
        }
    }
}

