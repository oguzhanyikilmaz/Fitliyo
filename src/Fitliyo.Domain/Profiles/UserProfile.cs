using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Profiles;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Profiles;

/// <summary>
/// Kullanıcı sağlık ve profil bilgisi — öğrenci ve eğitmen için ortak.
/// BMR, BMI, TDEE gibi metrikler bu verilerden hesaplanır.
/// </summary>
public class UserProfile : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    /// <summary>Doğum tarihi — yaş ve BMR hesabı için</summary>
    public DateTime? BirthDate { get; set; }

    /// <summary>Cinsiyet — BMR formülü için</summary>
    public Gender Gender { get; set; }

    /// <summary>Boy (cm)</summary>
    public decimal? HeightCm { get; set; }

    /// <summary>Mevcut kilo (kg)</summary>
    public decimal? WeightKg { get; set; }

    /// <summary>Kan grubu (A+, B+, vb.)</summary>
    [StringLength(UserProfileConsts.MaxBloodTypeLength)]
    public string? BloodType { get; set; }

    /// <summary>Aktivite düzeyi — TDEE çarpanı için</summary>
    public ActivityLevel ActivityLevel { get; set; }

    /// <summary>Hedef (kilo ver, kilo al, koru, vb.)</summary>
    public FitnessGoal FitnessGoal { get; set; }

    /// <summary>Kronik hastalıklar / rahatsızlıklar</summary>
    [StringLength(UserProfileConsts.MaxChronicConditionsLength)]
    public string? ChronicConditions { get; set; }

    /// <summary>Alerjiler</summary>
    [StringLength(UserProfileConsts.MaxAllergiesLength)]
    public string? Allergies { get; set; }

    /// <summary>Kullandığı ilaçlar</summary>
    [StringLength(UserProfileConsts.MaxMedicationsLength)]
    public string? Medications { get; set; }

    /// <summary>Bilinen sakatlıklar / yaralanmalar</summary>
    [StringLength(UserProfileConsts.MaxInjuriesLength)]
    public string? Injuries { get; set; }

    /// <summary>Acil durum iletişim</summary>
    [StringLength(UserProfileConsts.MaxEmergencyContactLength)]
    public string? EmergencyContact { get; set; }

    /// <summary>Profil için telefon (Identity’deki ayrı tutulabilir)</summary>
    [StringLength(UserProfileConsts.MaxPhoneLength)]
    public string? Phone { get; set; }

    /// <summary>Genel notlar</summary>
    [StringLength(UserProfileConsts.MaxNotesLength)]
    public string? Notes { get; set; }

    /// <summary>Bel çevresi (cm) — vücut yağ tahmini için</summary>
    public decimal? WaistCm { get; set; }

    /// <summary>Kalça çevresi (cm)</summary>
    public decimal? HipCm { get; set; }

    /// <summary>Boyun çevresi (cm) — Navy formülü için</summary>
    public decimal? NeckCm { get; set; }

    /// <summary>Hedef kilo (kg)</summary>
    public decimal? TargetWeightKg { get; set; }

    /// <summary>Ortalama uyku süresi (saat/gün)</summary>
    public int? SleepHoursPerNight { get; set; }

    /// <summary>Sigara kullanıyor mu</summary>
    public bool? Smoking { get; set; }

    /// <summary>Alkol tüketimi özeti</summary>
    [StringLength(UserProfileConsts.MaxAlcoholConsumptionLength)]
    public string? AlcoholConsumption { get; set; }

    /// <summary>Dinlenim kalp atışı (bpm)</summary>
    public int? RestingHeartRate { get; set; }

    /// <summary>Doktor / eğitmen notları (gizli alan)</summary>
    [StringLength(UserProfileConsts.MaxDoctorNotesLength)]
    public string? DoctorNotes { get; set; }

    protected UserProfile()
    {
    }

    public UserProfile(Guid id, Guid userId) : base(id)
    {
        UserId = userId;
    }
}
