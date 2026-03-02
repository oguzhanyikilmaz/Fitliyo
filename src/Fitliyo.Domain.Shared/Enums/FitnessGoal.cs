namespace Fitliyo.Enums;

/// <summary>
/// Fitness / sağlık hedefi.
/// </summary>
public enum FitnessGoal
{
    NotSpecified = 0,
    /// <summary>Kilo ver</summary>
    LoseWeight = 1,
    /// <summary>Kilo al / kas kütlesi</summary>
    GainMuscle = 2,
    /// <summary>Mevcut kiloyu koru</summary>
    Maintain = 3,
    /// <summary>Genel fitness / dayanıklılık</summary>
    GeneralFitness = 4,
    /// <summary>Performans artışı (sporcu)</summary>
    Performance = 5
}
