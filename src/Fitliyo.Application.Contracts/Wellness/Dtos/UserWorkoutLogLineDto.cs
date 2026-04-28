using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class UserWorkoutLogLineDto : EntityDto<Guid>
{
    public Guid UserWorkoutLogId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public decimal DurationMinutes { get; set; }
    public decimal Met { get; set; }
    public decimal CaloriesBurned { get; set; }
}
