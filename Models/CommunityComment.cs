using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models
{
    /// <summary>
    /// Represents a comment on a community post
    /// </summary>
    public class CommunityComment
    {
        public int CommentId { get; set; }

        /// <summary>
        /// Post ID this comment belongs to
        /// </summary>
        [Required]
        public int PostId { get; set; }

        /// <summary>
        /// User ID who created the comment
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Comment content
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Whether comment is anonymous
        /// </summary>
        public bool IsAnonymous { get; set; } = false;

        /// <summary>
        /// Number of likes
        /// </summary>
        public int LikesCount { get; set; } = 0;

        /// <summary>
        /// Timestamp when comment was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when comment was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual CommunityPost Post { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}

