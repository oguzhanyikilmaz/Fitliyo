using Fitliyo.Enums;
using Fitliyo.Wellness;
using Shouldly;
using Xunit;

namespace Fitliyo.Wellness;

public class WellnessMetabolicMathTests
{
    [Theory]
    [InlineData(17.0, BmiCategory.Underweight)]
    [InlineData(22.0, BmiCategory.Normal)]
    [InlineData(27.0, BmiCategory.Overweight)]
    [InlineData(32.0, BmiCategory.ObeseClass1)]
    [InlineData(36.0, BmiCategory.ObeseClass2)]
    [InlineData(42.0, BmiCategory.ObeseClass3)]
    public void BmiToCategory_Should_Classify_Who_Bands(decimal bmi, BmiCategory expected)
    {
        WellnessMetabolicMath.BmiToCategory(bmi).ShouldBe(expected);
    }

    [Fact]
    public void MetCalories_TypicalWalking_Should_Match_Formula()
    {
        var kcal = WellnessMetabolicMath.CalculateKcalFromMet(3.5m, 70m, 60m);
        kcal.ShouldBe(245);
    }

    [Fact]
    public void Kcal_Macros_For_100G_Portion()
    {
        WellnessMetabolicMath.CalculateKcalAndMacrosForPortion(
            200, 10, 20, 5, 150,
            out var kcal, out var p, out var c, out var f);
        kcal.ShouldBe(300);
        p.ShouldBe(15.0m);
    }
}
