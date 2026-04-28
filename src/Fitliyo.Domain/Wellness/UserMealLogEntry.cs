using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Günlük kayıt içindeki tek öğün kalemi.
/// </summary>
public class UserMealLogEntry : AuditedEntity<Guid>
{
    [Required]
    public Guid UserDailyMealLogId { get; set; }

    public MealType MealType { get; set; }

    public Guid? UserFoodItemId { get; set; }

    [StringLength(WellnessConsts.MaxFoodNameLength)]
    public string? FoodName { get; set; }

    public decimal PortionGrams { get; set; }

    public decimal Calories { get; set; }
    public decimal ProteinG { get; set; }
    public decimal CarbsG { get; set; }
    public decimal FatG { get; set; }

    public UserMealLogEntry(Guid id) : base(id)
    {
    }
}
