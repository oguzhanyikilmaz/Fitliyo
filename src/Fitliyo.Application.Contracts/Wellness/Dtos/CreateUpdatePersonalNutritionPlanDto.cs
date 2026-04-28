using System.ComponentModel.DataAnnotations;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class CreateUpdatePersonalNutritionPlanDto
{
    [Required]
    [StringLength(WellnessConsts.MaxTitleLength)]
    public string Title { get; set; } = string.Empty;

    [StringLength(WellnessConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Range(0, 20000)]
    public decimal? DailyCalorieTarget { get; set; }

    [Range(0, 1000)]
    public decimal? DailyProteinTargetG { get; set; }

    [Range(0, 2000)]
    public decimal? DailyCarbsTargetG { get; set; }

    [Range(0, 2000)]
    public decimal? DailyFatTargetG { get; set; }

    public bool IsActive { get; set; } = true;
}
