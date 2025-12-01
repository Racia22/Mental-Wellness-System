using MentalWellnessSystem.Models;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Service for AI-powered resource recommendations based on patient data
    /// </summary>
    public interface IRecommendationService
    {
        /// <summary>
        /// Generate recommendations based on patient's mood logs
        /// </summary>
        Task<List<ResourceRecommendation>> GenerateRecommendationsFromMoodLogsAsync(string patientId);

        /// <summary>
        /// Generate recommendations based on treatment plan
        /// </summary>
        Task<List<ResourceRecommendation>> GenerateRecommendationsFromTreatmentPlanAsync(int treatmentPlanId);

        /// <summary>
        /// Get all recommendations for a patient
        /// </summary>
        Task<List<ResourceRecommendation>> GetPatientRecommendationsAsync(string patientId, bool unviewedOnly = false);

        /// <summary>
        /// Mark a recommendation as viewed
        /// </summary>
        Task MarkAsViewedAsync(int recommendationId);
    }
}

