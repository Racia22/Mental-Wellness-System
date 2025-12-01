using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Models;

/// <summary>
/// Main database context for Mental Wellness System
/// Extends IdentityDbContext to include ApplicationUser and domain entities
/// </summary>
public class MentalWellnessDbContext : IdentityDbContext<ApplicationUser>
{
    public MentalWellnessDbContext(DbContextOptions<MentalWellnessDbContext> options)
        : base(options)
    {
    }

    // Domain entities
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<PatientRecord> PatientRecords { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    // Advanced features
    public DbSet<MoodLog> MoodLogs { get; set; }
    public DbSet<TreatmentPlan> TreatmentPlans { get; set; }
    public DbSet<TelehealthSession> TelehealthSessions { get; set; }
    public DbSet<PatientDocument> PatientDocuments { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<ResourceRecommendation> ResourceRecommendations { get; set; }
    public DbSet<CommunityPost> CommunityPosts { get; set; }
    public DbSet<CommunityComment> CommunityComments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ApplicationUser
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.PatientID)
                .HasMaxLength(50);

            entity.HasIndex(e => e.PatientID)
                .IsUnique()
                .HasFilter("[PatientID] IS NOT NULL");

            entity.Property(e => e.Phone)
                .HasMaxLength(20);

            entity.Property(e => e.Specialty)
                .HasMaxLength(100);

            entity.Property(e => e.Gender)
                .HasMaxLength(10);

            entity.Property(e => e.Category)
                .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // Configure Appointment
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId);

            entity.Property(e => e.AppointmentId)
                .HasColumnName("AppointmentID");

            entity.Property(e => e.PatientId)
                .IsRequired()
                .HasMaxLength(450); // Identity User ID length

            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.AppointmentType)
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Scheduled");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Appointment_Patient");

            entity.HasOne(d => d.Doctor)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Appointment_Doctor");

            // Index for efficient queries
            entity.HasIndex(e => new { e.DoctorId, e.AppointmentDate, e.AppointmentTime })
                .HasDatabaseName("IX_Appointment_DoctorDateTime");

            entity.HasIndex(e => new { e.PatientId, e.AppointmentDate })
                .HasDatabaseName("IX_Appointment_PatientDate");
        });

        // Configure PatientRecord
        modelBuilder.Entity<PatientRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId);

            entity.Property(e => e.RecordId)
                .HasColumnName("RecordID");

            entity.Property(e => e.PatientId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.DoctorId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.Diagnosis)
                .HasMaxLength(500);

            entity.Property(e => e.FollowUpRequest)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PatientRecord_Patient");

            entity.HasOne(d => d.Doctor)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PatientRecord_Doctor");

            entity.HasOne(d => d.Appointment)
                .WithMany()
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PatientRecord_Appointment");

            // Index for efficient queries
            entity.HasIndex(e => e.PatientId)
                .HasDatabaseName("IX_PatientRecord_Patient");

            entity.HasIndex(e => e.DoctorId)
                .HasDatabaseName("IX_PatientRecord_Doctor");
        });

        // Configure Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId);

            entity.Property(e => e.NotificationId)
                .HasColumnName("NotificationID");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.NotificationType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Message)
                .IsRequired();

            entity.Property(e => e.DeliveryMethod)
                .HasMaxLength(20)
                .HasDefaultValue("Email");

            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Notification_User");

            entity.HasOne(d => d.Appointment)
                .WithMany()
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Notification_Appointment");

            // Indexes
            entity.HasIndex(e => new { e.UserId, e.Status })
                .HasDatabaseName("IX_Notification_UserStatus");

            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Notification_CreatedAt");
        });

        // Configure AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditLogId);

            entity.Property(e => e.AuditLogId)
                .HasColumnName("AuditLogID");

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.Action)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.EntityType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.IpAddress)
                .HasMaxLength(45); // IPv6 max length

            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_AuditLog_User");

            // Indexes
            entity.HasIndex(e => new { e.UserId, e.Timestamp })
                .HasDatabaseName("IX_AuditLog_UserTimestamp");

            entity.HasIndex(e => new { e.EntityType, e.EntityId })
                .HasDatabaseName("IX_AuditLog_Entity");
        });

        // Configure MoodLog
        modelBuilder.Entity<MoodLog>(entity =>
        {
            entity.HasKey(e => e.MoodLogId);
            entity.Property(e => e.PatientId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.Property(e => e.SleepHours).HasPrecision(4, 1);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_MoodLog_Patient");

            entity.HasIndex(e => new { e.PatientId, e.LogDate })
                .HasDatabaseName("IX_MoodLog_PatientDate");
        });

        // Configure TreatmentPlan
        modelBuilder.Entity<TreatmentPlan>(entity =>
        {
            entity.HasKey(e => e.TreatmentPlanId);
            entity.Property(e => e.PatientId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.DoctorId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Goals).HasMaxLength(1000);
            entity.Property(e => e.Tasks).HasMaxLength(2000);
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Active");
            entity.Property(e => e.ProgressNotes).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TreatmentPlan_Patient");

            entity.HasOne(d => d.Doctor)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TreatmentPlan_Doctor");

            entity.HasOne(d => d.Appointment)
                .WithMany()
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TreatmentPlan_Appointment");

            entity.HasOne(d => d.PatientRecord)
                .WithMany()
                .HasForeignKey(d => d.PatientRecordId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TreatmentPlan_PatientRecord");

            entity.HasIndex(e => e.PatientId).HasDatabaseName("IX_TreatmentPlan_Patient");
            entity.HasIndex(e => e.DoctorId).HasDatabaseName("IX_TreatmentPlan_Doctor");
        });

        // Configure TelehealthSession
        modelBuilder.Entity<TelehealthSession>(entity =>
        {
            entity.HasKey(e => e.TelehealthSessionId);
            entity.Property(e => e.AppointmentId).IsRequired();
            entity.Property(e => e.SessionUrl).HasMaxLength(500);
            entity.Property(e => e.SessionType).HasMaxLength(20).HasDefaultValue("Video");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Scheduled");
            entity.Property(e => e.SessionNotes).HasMaxLength(2000);
            entity.Property(e => e.RecordingUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Appointment)
                .WithMany()
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TelehealthSession_Appointment");

            entity.HasIndex(e => e.AppointmentId).HasDatabaseName("IX_TelehealthSession_Appointment");
        });

        // Configure PatientDocument
        modelBuilder.Entity<PatientDocument>(entity =>
        {
            entity.HasKey(e => e.DocumentId);
            entity.Property(e => e.PatientId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.DoctorId).HasMaxLength(450);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DocumentType).HasMaxLength(50).HasDefaultValue("Other");
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.MimeType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_PatientDocument_Patient");

            entity.HasOne(d => d.Doctor)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PatientDocument_Doctor");

            entity.HasOne(d => d.Appointment)
                .WithMany()
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PatientDocument_Appointment");

            entity.HasOne(d => d.PatientRecord)
                .WithMany()
                .HasForeignKey(d => d.PatientRecordId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PatientDocument_PatientRecord");

            entity.HasIndex(e => e.PatientId).HasDatabaseName("IX_PatientDocument_Patient");
        });

        // Configure Feedback
        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId);
            entity.Property(e => e.PatientId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.DoctorId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.AppointmentId).IsRequired();
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comments).HasMaxLength(1000);
            entity.Property(e => e.SubmittedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Feedback_Patient");

            entity.HasOne(d => d.Doctor)
                .WithMany()
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Feedback_Doctor");

            entity.HasOne(d => d.Appointment)
                .WithMany()
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Feedback_Appointment");

            entity.HasIndex(e => e.DoctorId).HasDatabaseName("IX_Feedback_Doctor");
            
            // Unique constraint: one feedback per appointment (also serves as index)
            entity.HasIndex(e => e.AppointmentId)
                .IsUnique()
                .HasDatabaseName("IX_Feedbacks_AppointmentId_Unique");
        });

        // Configure ResourceRecommendation
        modelBuilder.Entity<ResourceRecommendation>(entity =>
        {
            entity.HasKey(e => e.RecommendationId);
            entity.Property(e => e.PatientId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ResourceType).HasMaxLength(50).HasDefaultValue("Article");
            entity.Property(e => e.ResourceUrl).HasMaxLength(500);
            entity.Property(e => e.RecommendationReason).HasMaxLength(500);
            entity.Property(e => e.Priority).HasMaxLength(20).HasDefaultValue("Medium");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ResourceRecommendation_Patient");

            entity.HasOne(d => d.MoodLog)
                .WithMany()
                .HasForeignKey(d => d.MoodLogId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ResourceRecommendation_MoodLog");

            entity.HasOne(d => d.TreatmentPlan)
                .WithMany()
                .HasForeignKey(d => d.TreatmentPlanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ResourceRecommendation_TreatmentPlan");

            entity.HasIndex(e => e.PatientId).HasDatabaseName("IX_ResourceRecommendation_Patient");
        });

        // Configure CommunityPost
        modelBuilder.Entity<CommunityPost>(entity =>
        {
            entity.HasKey(e => e.PostId);
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(50).HasDefaultValue("General");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CommunityPost_User");

            entity.HasIndex(e => e.Category).HasDatabaseName("IX_CommunityPost_Category");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_CommunityPost_CreatedAt");
        });

        // Configure CommunityComment
        modelBuilder.Entity<CommunityComment>(entity =>
        {
            entity.HasKey(e => e.CommentId);
            entity.Property(e => e.PostId).IsRequired();
            entity.Property(e => e.UserId).IsRequired().HasMaxLength(450);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(d => d.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_CommunityComment_Post");

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CommunityComment_User");

            entity.HasIndex(e => e.PostId).HasDatabaseName("IX_CommunityComment_Post");
        });
    }
}
