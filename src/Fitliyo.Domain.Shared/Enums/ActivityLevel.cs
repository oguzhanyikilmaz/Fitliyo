namespace Fitliyo.Enums;

/// <summary>
/// Günlük aktivite düzeyi — TDEE (günlük kalori ihtiyacı) hesaplamasında kullanılır.
/// </summary>
public enum ActivityLevel
{
    NotSpecified = 0,
    /// <summary>Hareketsiz (oturarak çalışan)</summary>
    Sedentary = 1,
    /// <summary>Hafif (haftada 1-3 gün hafif egzersiz)</summary>
    Light = 2,
    /// <summary>Orta (haftada 3-5 gün orta egzersiz)</summary>
    Moderate = 3,
    /// <summary>Aktif (haftada 6-7 gün yoğun egzersiz)</summary>
    Active = 4,
    /// <summary>Çok aktif (fiziksel iş + yoğun antrenman)</summary>
    VeryActive = 5
}
