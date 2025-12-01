using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MentalWellnessSystem.Models;

/// <summary>
/// Represents a patient record/medical note created by a doctor
/// </summary>
public class PatientRecord
{
    public int RecordId { get; set; }

    /// <summary>
    /// Patient User ID (from Identity)
    /// </summary>
    [Required]
    public string PatientId { get; set; } = string.Empty;

    /// <summary>
    /// Doctor User ID (from Identity) who created this record
    /// </summary>
    [Required]
    public string DoctorId { get; set; } = string.Empty;

    /// <summary>
    /// Session notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Diagnosis information
    /// </summary>
    [MaxLength(500)]
    public string? Diagnosis { get; set; }

    /// <summary>
    /// Follow-up recommendations
    /// </summary>
    [MaxLength(500)]
    public string? FollowUpRequest { get; set; }

    /// <summary>
    /// Related appointment ID (if created from an appointment)
    /// </summary>
    public int? AppointmentId { get; set; }

    /// <summary>
    /// Timestamp when record was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ApplicationUser Patient { get; set; } = null!;
    public virtual ApplicationUser Doctor { get; set; } = null!;
    public virtual Appointment? Appointment { get; set; }
}
