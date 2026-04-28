using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class PersonalWorkoutProgramDto : AuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? WeekdayIndex { get; set; }
    public bool IsArchived { get; set; }
    public List<PersonalWorkoutTemplateExerciseDto> Exercises { get; set; } = new();
}
