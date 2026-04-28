namespace Fitliyo.Enums;

/// <summary>
/// WHO sınıflandırmasına göre VKİ kategorisi (özet).
/// </summary>
public enum BmiCategory
{
    /// <summary>Boy/kilo yetersiz</summary>
    Unknown = 0,

    /// <summary>VKİ &lt; 18.5</summary>
    Underweight = 1,

    /// <summary>18.5–24.9</summary>
    Normal = 2,

    /// <summary>25–29.9</summary>
    Overweight = 3,

    /// <summary>Obezite sınıf I — 30–34.9</summary>
    ObeseClass1 = 4,

    /// <summary>Obezite sınıf II — 35–39.9</summary>
    ObeseClass2 = 5,

    /// <summary>Obezite sınıf III — ≥ 40</summary>
    ObeseClass3 = 6
}
