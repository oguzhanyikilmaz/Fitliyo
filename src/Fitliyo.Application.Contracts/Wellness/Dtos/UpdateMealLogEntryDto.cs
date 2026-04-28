using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class UpdateMealLogEntryDto
{
    public MealType MealType { get; set; }

    public Guid? UserFoodItemId { get; set; }

    [StringLength(WellnessConsts.MaxFoodNameLength)]
    public string? FoodName { get; set; }

    [Range(0.1, 100000)]
    public decimal PortionGrams { get; set; }

    [Range(0, 10000)]
    public decimal? KcalPer100GManual { get; set; }

    [Range(0, 1000)]
    public decimal? ProteinPer100GManual { get; set; }

    [Range(0, 1000)]
    public decimal? CarbsPer100GManual { get; set; }

    [Range(0, 1000)]
    public decimal? FatPer100GManual { get; set; }
}
