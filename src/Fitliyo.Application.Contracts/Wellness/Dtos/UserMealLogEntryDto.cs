using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class UserMealLogEntryDto : EntityDto<Guid>
{
    public Guid UserDailyMealLogId { get; set; }
    public MealType MealType { get; set; }
    public Guid? UserFoodItemId { get; set; }
    public string? FoodName { get; set; }
    public decimal PortionGrams { get; set; }
    public decimal Calories { get; set; }
    public decimal ProteinG { get; set; }
    public decimal CarbsG { get; set; }
    public decimal FatG { get; set; }
}
