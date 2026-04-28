using System;
using Fitliyo.Enums;

namespace Fitliyo.Wellness;

/// <summary>
/// VKİ kategorisi ve MET tabanlı kalori (saf matematik, UI ve servislerde ortak).
/// Kcal = MET × kg × süre(saat) — Compendium yaklaşımı.
/// </summary>
public static class WellnessMetabolicMath
{
    public static BmiCategory BmiToCategory(decimal bmi)
    {
        if (bmi < 18.5m) return BmiCategory.Underweight;
        if (bmi < 25m) return BmiCategory.Normal;
        if (bmi < 30m) return BmiCategory.Overweight;
        if (bmi < 35m) return BmiCategory.ObeseClass1;
        if (bmi < 40m) return BmiCategory.ObeseClass2;
        return BmiCategory.ObeseClass3;
    }

    /// <summary>Türkçe kısa açıklama (UI için).</summary>
    public static string GetBmiCategoryDescriptionTr(BmiCategory category)
    {
        return category switch
        {
            BmiCategory.Underweight => "Zayıf (VKİ 18,5 altı)",
            BmiCategory.Normal => "Normal kilo (VKİ 18,5–24,9)",
            BmiCategory.Overweight => "Fazla kilolu (VKİ 25–29,9)",
            BmiCategory.ObeseClass1 => "Obezite sınıf I (VKİ 30–34,9)",
            BmiCategory.ObeseClass2 => "Obezite sınıf II (VKİ 35–39,9)",
            BmiCategory.ObeseClass3 => "Obezite sınıf III (VKİ 40+)",
            _ => "Hesaplanamadı — boy ve kilo giriniz."
        };
    }

    public static bool IsObese(BmiCategory category) =>
        category is BmiCategory.ObeseClass1 or BmiCategory.ObeseClass2 or BmiCategory.ObeseClass3;

    /// <summary>Harcanan kcal: MET × ağırlık(kg) × saat.</summary>
    public static decimal CalculateKcalFromMet(decimal met, decimal weightKg, decimal durationMinutes)
    {
        if (met <= 0 || weightKg <= 0 || durationMinutes <= 0) return 0;
        var hours = durationMinutes / 60m;
        return Math.Round(met * weightKg * hours, 0, MidpointRounding.AwayFromZero);
    }

    /// <summary>100 g için makro değerlerden porsiyon kalorisi (Atwater yaklaşık: 4-4-9).</summary>
    public static void CalculateKcalAndMacrosForPortion(
        decimal kcalPer100g,
        decimal proteinPer100g,
        decimal carbsPer100g,
        decimal fatPer100g,
        decimal portionGrams,
        out decimal kcal,
        out decimal proteinG,
        out decimal carbsG,
        out decimal fatG)
    {
        if (portionGrams <= 0)
        {
            kcal = proteinG = carbsG = fatG = 0;
            return;
        }

        var f = portionGrams / 100m;
        kcal = Math.Round((kcalPer100g > 0 ? kcalPer100g : (proteinPer100g * 4m + carbsPer100g * 4m + fatPer100g * 9m)) * f, 0, MidpointRounding.AwayFromZero);
        proteinG = Math.Round(proteinPer100g * f, 1, MidpointRounding.AwayFromZero);
        carbsG = Math.Round(carbsPer100g * f, 1, MidpointRounding.AwayFromZero);
        fatG = Math.Round(fatPer100g * f, 1, MidpointRounding.AwayFromZero);
    }
}
