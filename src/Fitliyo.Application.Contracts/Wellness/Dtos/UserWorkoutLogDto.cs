using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class UserWorkoutLogDto : AuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public Guid? PersonalWorkoutProgramId { get; set; }
    public DateTime LogDate { get; set; }
    public string? Notes { get; set; }
    public decimal TotalCaloriesBurned { get; set; }
    public List<UserWorkoutLogLineDto> Lines { get; set; } = new();
}
