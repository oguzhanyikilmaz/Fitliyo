using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class PersonalNutritionPlanDto : AuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DailyCalorieTarget { get; set; }
    public decimal? DailyProteinTargetG { get; set; }
    public decimal? DailyCarbsTargetG { get; set; }
    public decimal? DailyFatTargetG { get; set; }
    public bool IsActive { get; set; }
}
