using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Antrenman günlüğünde yapılan tek egzersiz satırı.
/// </summary>
public class UserWorkoutLogLine : AuditedEntity<Guid>
{
    [Required]
    public Guid UserWorkoutLogId { get; set; }

    [Required]
    [StringLength(WellnessConsts.MaxExerciseNameLength)]
    public string ExerciseName { get; set; } = string.Empty;

    public decimal DurationMinutes { get; set; }

    /// <summary>Compendium MET değeri</summary>
    public decimal Met { get; set; }

    /// <summary>Hesaplanan kcal</summary>
    public decimal CaloriesBurned { get; set; }

    public UserWorkoutLogLine(Guid id) : base(id)
    {
    }
}
