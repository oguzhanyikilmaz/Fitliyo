using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Profiles;

namespace Fitliyo.Profiles.Dtos;

public class CreateUpdateUserProfileDto
{
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }

    [StringLength(UserProfileConsts.MaxBloodTypeLength)]
    public string? BloodType { get; set; }

    public ActivityLevel ActivityLevel { get; set; }
    public FitnessGoal FitnessGoal { get; set; }

    [StringLength(UserProfileConsts.MaxChronicConditionsLength)]
    public string? ChronicConditions { get; set; }

    [StringLength(UserProfileConsts.MaxAllergiesLength)]
    public string? Allergies { get; set; }

    [StringLength(UserProfileConsts.MaxMedicationsLength)]
    public string? Medications { get; set; }

    [StringLength(UserProfileConsts.MaxInjuriesLength)]
    public string? Injuries { get; set; }

    [StringLength(UserProfileConsts.MaxEmergencyContactLength)]
    public string? EmergencyContact { get; set; }

    [StringLength(UserProfileConsts.MaxPhoneLength)]
    public string? Phone { get; set; }

    [StringLength(UserProfileConsts.MaxNotesLength)]
    public string? Notes { get; set; }

    public decimal? WaistCm { get; set; }
    public decimal? HipCm { get; set; }
    public decimal? NeckCm { get; set; }
    public decimal? TargetWeightKg { get; set; }
    public int? SleepHoursPerNight { get; set; }
    public bool? Smoking { get; set; }

    [StringLength(UserProfileConsts.MaxAlcoholConsumptionLength)]
    public string? AlcoholConsumption { get; set; }

    public int? RestingHeartRate { get; set; }
}
