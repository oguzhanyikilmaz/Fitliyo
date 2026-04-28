using System;

namespace Fitliyo.Wellness.Dtos;

/// <summary>
/// Seçilen gün için besin, antrenman ve hedef özeti.
/// </summary>
public class DailyWellnessSummaryDto
{
    public DateTime Date { get; set; }

    public decimal TotalCaloriesConsumed { get; set; }
    public decimal TotalProteinG { get; set; }
    public decimal TotalCarbsG { get; set; }
    public decimal TotalFatG { get; set; }

    public decimal TotalCaloriesBurnedFromWorkouts { get; set; }

    public decimal? Tdee { get; set; }
    public decimal? Bmr { get; set; }

    public string? ActiveNutritionPlanTitle { get; set; }
    public decimal? ReferenceCalorieTarget { get; set; }

    /// <summary>
    /// Tahmini kalan bütçe: hedef (plan veya TDEE) - alınan + antrenmanda harcanan.
    /// </summary>
    public decimal? RemainingCalorieBudget { get; set; }
}
