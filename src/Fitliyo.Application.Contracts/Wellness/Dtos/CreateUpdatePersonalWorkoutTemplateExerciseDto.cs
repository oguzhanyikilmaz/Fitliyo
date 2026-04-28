using System.ComponentModel.DataAnnotations;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class CreateUpdatePersonalWorkoutTemplateExerciseDto
{
    public int DayNumber { get; set; } = 1;

    public int SortOrder { get; set; }

    [Required]
    [StringLength(WellnessConsts.MaxExerciseNameLength)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1000)]
    public int? TargetSets { get; set; }

    [Range(0, 100000)]
    public int? TargetReps { get; set; }

    [Range(0, 24 * 60)]
    public int? SuggestedDurationMinutes { get; set; }

    [Range(0.1, 25)]
    public decimal? DefaultMet { get; set; }

    [StringLength(WellnessConsts.MaxTemplateExerciseNoteLength)]
    public string? Notes { get; set; }
}
