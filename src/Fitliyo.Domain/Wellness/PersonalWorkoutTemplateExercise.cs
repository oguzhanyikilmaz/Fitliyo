using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Program şablonundaki tek hareket satırı.
/// </summary>
public class PersonalWorkoutTemplateExercise : AuditedEntity<Guid>
{
    [Required]
    public Guid PersonalWorkoutProgramId { get; set; }

    /// <summary>Aynı programda sıra (1 tabanlı hafta günü veya blok)</summary>
    public int DayNumber { get; set; } = 1;

    public int SortOrder { get; set; }

    [Required]
    [StringLength(WellnessConsts.MaxExerciseNameLength)]
    public string Name { get; set; } = string.Empty;

    public int? TargetSets { get; set; }
    public int? TargetReps { get; set; }

    /// <summary>Önerilen süre (dk)</summary>
    public int? SuggestedDurationMinutes { get; set; }

    /// <summary>Önerilen MET (tahmini enerji, şablon)</summary>
    public decimal? DefaultMet { get; set; }

    [StringLength(WellnessConsts.MaxTemplateExerciseNoteLength)]
    public string? Notes { get; set; }

    public PersonalWorkoutTemplateExercise(Guid id) : base(id)
    {
    }
}
