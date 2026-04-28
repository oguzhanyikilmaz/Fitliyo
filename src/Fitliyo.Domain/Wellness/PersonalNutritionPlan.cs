using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Kullanıcıya özel beslenme hedefi (günlük kalori + makro).
/// </summary>
public class PersonalNutritionPlan : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    [Required]
    [StringLength(WellnessConsts.MaxTitleLength)]
    public string Title { get; set; } = string.Empty;

    [StringLength(WellnessConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    public decimal? DailyCalorieTarget { get; set; }
    public decimal? DailyProteinTargetG { get; set; }
    public decimal? DailyCarbsTargetG { get; set; }
    public decimal? DailyFatTargetG { get; set; }

    public bool IsActive { get; set; } = true;

    protected PersonalNutritionPlan()
    {
    }

    public PersonalNutritionPlan(Guid id, Guid userId, string title) : base(id)
    {
        UserId = userId;
        Title = title;
    }
}
