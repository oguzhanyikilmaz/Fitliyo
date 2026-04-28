using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class CreateMealLogEntryDto
{
    public MealType MealType { get; set; }

    /// <summary>Kayıtlı gıda; doluysa 100g değerleri oradan alınır.</summary>
    public Guid? UserFoodItemId { get; set; }

    [StringLength(WellnessConsts.MaxFoodNameLength)]
    public string? FoodName { get; set; }

    [Range(0.1, 100000)]
    public decimal PortionGrams { get; set; }

    /// <summary>Kayıtlı gıda yoksa zorunlu: 100g başına kcal (etiket).</summary>
    [Range(0, 10000)]
    public decimal? KcalPer100GManual { get; set; }

    [Range(0, 1000)]
    public decimal? ProteinPer100GManual { get; set; }

    [Range(0, 1000)]
    public decimal? CarbsPer100GManual { get; set; }

    [Range(0, 1000)]
    public decimal? FatPer100GManual { get; set; }
}
