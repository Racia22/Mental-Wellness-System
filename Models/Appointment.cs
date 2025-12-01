using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models;

/// <summary>
/// Represents an appointment between a patient and a doctor
/// </summary>
public class Appointment
{
    public int AppointmentId { get; set; }

    /// <summary>
    /// Patient User ID (from Identity)
    /// </summary>
    [Required]
    public string PatientId { get; set; } = string.Empty;

    /// <summary>
    /// Doctor User ID (from Identity)
    /// </summary>
    [Required]
    public string DoctorId { get; set; } = string.Empty;

    /// <summary>
    /// Appointment date
    /// </summary>
    [Required]
    public DateOnly AppointmentDate { get; set; }

    /// <summary>
    /// Appointment time
    /// </summary>
    [Required]
    public TimeOnly AppointmentTime { get; set; }

    /// <summary>
    /// Type of appointment (e.g., "Initial Consultation", "Follow-up", "Therapy Session")
    /// </summary>
    [MaxLength(50)]
    public string? AppointmentType { get; set; }

    /// <summary>
    /// Status: Scheduled, Ongoing, Completed, Cancelled
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Scheduled";

    /// <summary>
    /// Notes added by doctor
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Timestamp when appointment was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when appointment was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser Patient { get; set; } = null!;
    public virtual ApplicationUser Doctor { get; set; } = null!;
}
