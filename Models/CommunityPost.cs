using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents a community forum post for peer support
    /// </summary>
    public class CommunityPost
    {
        public int PostId { get; set; }

        /// <summary>
        /// User ID who created the post
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Post title
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Post content
        /// </summary>
        [Required]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Post category: General, Support, Questions, Success Stories
        /// </summary>
        [MaxLength(50)]
        public string Category { get; set; } = "General";

        /// <summary>
        /// Whether post is anonymous
        /// </summary>
        public bool IsAnonymous { get; set; } = false;

        /// <summary>
        /// Number of likes
        /// </summary>
        public int LikesCount { get; set; } = 0;

        /// <summary>
        /// Number of comments
        /// </summary>
        public int CommentsCount { get; set; } = 0;

        /// <summary>
        /// Whether post is pinned
        /// </summary>
        public bool IsPinned { get; set; } = false;

        /// <summary>
        /// Timestamp when post was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when post was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<CommunityComment> Comments { get; set; } = new List<CommunityComment>();
    }
}

