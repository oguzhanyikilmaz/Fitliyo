using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class CreateUpdatePersonalWorkoutProgramDto
{
    [Required]
    [StringLength(WellnessConsts.MaxTitleLength)]
    public string Title { get; set; } = string.Empty;

    [StringLength(WellnessConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    /// <summary>0–6: Pazar=0 … Cumartesi=6 veya null</summary>
    [Range(0, 6)]
    public int? WeekdayIndex { get; set; }

    public bool IsArchived { get; set; }

    public List<CreateUpdatePersonalWorkoutTemplateExerciseDto> Exercises { get; set; } = new();
}
