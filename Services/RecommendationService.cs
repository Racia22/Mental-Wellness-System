using MentalWellnessSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Services
{
    /// <summary>
    /// Implementation of AI-powered recommendation service
    /// Uses patient mood logs, treatment plans, and patterns to suggest resources
    /// </summary>
    public class RecommendationService : IRecommendationService
    {
        private readonly MentalWellnessDbContext _context;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(
            MentalWellnessDbContext context,
            ILogger<RecommendationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ResourceRecommendation>> GenerateRecommendationsFromMoodLogsAsync(string patientId)
        {
            var recommendations = new List<ResourceRecommendation>();

            // Get recent mood logs (last 7 days)
            var recentLogs = await _context.MoodLogs
                .Where(m => m.PatientId == patientId && m.LogDate >= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)))
                .OrderByDescending(m => m.LogDate)
                .ToListAsync();

            if (!recentLogs.Any())
                return recommendations;

            // Calculate average mood score
            var avgMood = recentLogs.Average(m => m.MoodScore);
            var avgStress = recentLogs.Where(m => m.StressLevel.HasValue).Average(m => m.StressLevel!.Value);
            var avgSleep = recentLogs.Where(m => m.SleepHours.HasValue).Average(m => m.SleepHours!.Value);

            // Generate recommendations based on patterns
            if (avgMood < 5)
            {
                recommendations.Add(new ResourceRecommendation
                {
                    PatientId = patientId,
                    Title = "Coping Strategies for Low Mood",
                    Description = "Resources to help improve your mood and mental well-being",
                    ResourceType = "Article",
                    ResourceUrl = "/resources/coping-strategies",
                    RecommendationReason = $"Based on your recent mood logs (average score: {avgMood:F1}/10)",
                    Priority = "High",
                    MoodLogId = recentLogs.First().MoodLogId
                });
            }

            if (avgStress > 7)
            {
                recommendations.Add(new ResourceRecommendation
                {
                    PatientId = patientId,
                    Title = "Stress Management Techniques",
                    Description = "Guided exercises to help manage stress levels",
                    ResourceType = "Exercise",
                    ResourceUrl = "/resources/stress-management",
                    RecommendationReason = $"Your stress levels have been elevated (average: {avgStress:F1}/10)",
                    Priority = "High"
                });
            }

            if (avgSleep < 6)
            {
                recommendations.Add(new ResourceRecommendation
                {
                    PatientId = patientId,
                    Title = "Sleep Hygiene Guide",
                    Description = "Tips and techniques for better sleep",
                    ResourceType = "Article",
                    ResourceUrl = "/resources/sleep-hygiene",
                    RecommendationReason = $"Your average sleep is {avgSleep:F1} hours, below recommended levels",
                    Priority = "Medium"
                });
            }

            // Add general wellness resources
            recommendations.Add(new ResourceRecommendation
            {
                PatientId = patientId,
                Title = "Daily Mindfulness Meditation",
                Description = "10-minute guided meditation for daily practice",
                ResourceType = "Meditation",
                ResourceUrl = "/resources/mindfulness-meditation",
                RecommendationReason = "Regular mindfulness practice can improve overall well-being",
                Priority = "Medium"
            });

            // Save recommendations
            await _context.ResourceRecommendations.AddRangeAsync(recommendations);
            await _context.SaveChangesAsync();

            return recommendations;
        }

        public async Task<List<ResourceRecommendation>> GenerateRecommendationsFromTreatmentPlanAsync(int treatmentPlanId)
        {
            var plan = await _context.TreatmentPlans
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(t => t.TreatmentPlanId == treatmentPlanId);

            if (plan == null)
                return new List<ResourceRecommendation>();

            var recommendations = new List<ResourceRecommendation>
            {
                new ResourceRecommendation
                {
                    PatientId = plan.PatientId,
                    Title = $"Resources for: {plan.Title}",
                    Description = plan.Description ?? "Supporting resources for your treatment plan",
                    ResourceType = "Article",
                    ResourceUrl = "/resources/treatment-support",
                    RecommendationReason = $"Recommended based on your active treatment plan",
                    Priority = "High",
                    TreatmentPlanId = treatmentPlanId
                }
            };

            await _context.ResourceRecommendations.AddRangeAsync(recommendations);
            await _context.SaveChangesAsync();

            return recommendations;
        }

        public async Task<List<ResourceRecommendation>> GetPatientRecommendationsAsync(string patientId, bool unviewedOnly = false)
        {
            var query = _context.ResourceRecommendations
                .Include(r => r.TreatmentPlan)
                    .ThenInclude(tp => tp.Doctor)
                .Where(r => r.PatientId == patientId);

            if (unviewedOnly)
                query = query.Where(r => !r.IsViewed);

            return await query
                .OrderByDescending(r => r.Priority == "High")
                .ThenByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsViewedAsync(int recommendationId)
        {
            var recommendation = await _context.ResourceRecommendations
                .FirstOrDefaultAsync(r => r.RecommendationId == recommendationId);

            if (recommendation != null)
            {
                recommendation.IsViewed = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}

