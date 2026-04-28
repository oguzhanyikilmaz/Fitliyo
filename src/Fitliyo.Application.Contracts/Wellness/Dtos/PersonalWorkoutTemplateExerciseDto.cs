using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class PersonalWorkoutTemplateExerciseDto : EntityDto<Guid>
{
    public Guid PersonalWorkoutProgramId { get; set; }
    public int DayNumber { get; set; }
    public int SortOrder { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? TargetSets { get; set; }
    public int? TargetReps { get; set; }
    public int? SuggestedDurationMinutes { get; set; }
    public decimal? DefaultMet { get; set; }
    public string? Notes { get; set; }
}
