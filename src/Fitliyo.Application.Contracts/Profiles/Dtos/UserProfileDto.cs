using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Profiles.Dtos;

/// <summary>
/// Kullanıcı profil DTO — hesaplanan metrikler dahil.
/// </summary>
public class UserProfileDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }

    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public string? BloodType { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
    public FitnessGoal FitnessGoal { get; set; }
    public string? ChronicConditions { get; set; }
    public string? Allergies { get; set; }
    public string? Medications { get; set; }
    public string? Injuries { get; set; }
    public string? EmergencyContact { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
    public decimal? WaistCm { get; set; }
    public decimal? HipCm { get; set; }
    public decimal? NeckCm { get; set; }
    public decimal? TargetWeightKg { get; set; }
    public int? SleepHoursPerNight { get; set; }
    public bool? Smoking { get; set; }
    public string? AlcoholConsumption { get; set; }
    public int? RestingHeartRate { get; set; }

    // ----- Hesaplanan (backend tarafından doldurulur) -----
    /// <summary>Yaş (BirthDate'den)</summary>
    public int? Age { get; set; }
    /// <summary>Vücut Kitle İndeksi</summary>
    public decimal? Bmi { get; set; }
    /// <summary>Bazal metabolizma hızı (kcal/gün)</summary>
    public decimal? Bmr { get; set; }
    /// <summary>Günlük toplam enerji harcaması (kcal/gün)</summary>
    public decimal? Tdee { get; set; }
    /// <summary>BMI 18.5'a göre ideal min kilo (kg)</summary>
    public decimal? IdealWeightMinKg { get; set; }
    /// <summary>BMI 24.9'a göre ideal max kilo (kg)</summary>
    public decimal? IdealWeightMaxKg { get; set; }
    /// <summary>Vücut yağ yüzdesi (Navy formülü, varsa)</summary>
    public decimal? BodyFatPercentage { get; set; }
}
