using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents AI-generated resource recommendations for patients
    /// </summary>
    public class ResourceRecommendation
    {
        public int RecommendationId { get; set; }

        /// <summary>
        /// Patient User ID
        /// </summary>
        [Required]
        public string PatientId { get; set; } = string.Empty;

        /// <summary>
        /// Resource title
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Resource description
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Resource type: Article, Exercise, Meditation, Video, Other
        /// </summary>
        [MaxLength(50)]
        public string ResourceType { get; set; } = "Article";

        /// <summary>
        /// Resource URL or content
        /// </summary>
        [MaxLength(500)]
        public string? ResourceUrl { get; set; }

        /// <summary>
        /// Reason for recommendation (AI-generated)
        /// </summary>
        [MaxLength(500)]
        public string? RecommendationReason { get; set; }

        /// <summary>
        /// Whether patient has viewed the resource
        /// </summary>
        public bool IsViewed { get; set; } = false;

        /// <summary>
        /// Priority level: High, Medium, Low
        /// </summary>
        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// Related mood log ID (if recommended based on mood)
        /// </summary>
        public int? MoodLogId { get; set; }

        /// <summary>
        /// Related treatment plan ID (if recommended based on treatment)
        /// </summary>
        public int? TreatmentPlanId { get; set; }

        /// <summary>
        /// Timestamp when recommendation was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser Patient { get; set; } = null!;
        public virtual MoodLog? MoodLog { get; set; }
        public virtual TreatmentPlan? TreatmentPlan { get; set; }
    }
}

