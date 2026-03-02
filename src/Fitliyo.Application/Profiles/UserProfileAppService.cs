using System;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Profiles.Dtos;
using Fitliyo;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Profiles;

[Authorize]
public class UserProfileAppService : FitliyoAppService, IUserProfileAppService
{
    private readonly IRepository<UserProfile, Guid> _repository;

    public UserProfileAppService(IRepository<UserProfile, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileDto> GetMyProfileAsync()
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var profile = await _repository.FirstOrDefaultAsync(x => x.UserId == userId);
        if (profile == null)
            return new UserProfileDto { UserId = userId };

        var dto = ObjectMapper.Map<UserProfile, UserProfileDto>(profile);
        FillCalculatedFields(dto);
        return dto;
    }

    public async Task<UserProfileDto> UpdateMyProfileAsync(CreateUpdateUserProfileDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var profile = await _repository.FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile(GuidGenerator.Create(), userId);
            await _repository.InsertAsync(profile);
        }

        profile.BirthDate = input.BirthDate;
        profile.Gender = input.Gender;
        profile.HeightCm = input.HeightCm;
        profile.WeightKg = input.WeightKg;
        profile.BloodType = input.BloodType;
        profile.ActivityLevel = input.ActivityLevel;
        profile.FitnessGoal = input.FitnessGoal;
        profile.ChronicConditions = input.ChronicConditions;
        profile.Allergies = input.Allergies;
        profile.Medications = input.Medications;
        profile.Injuries = input.Injuries;
        profile.EmergencyContact = input.EmergencyContact;
        profile.Phone = input.Phone;
        profile.Notes = input.Notes;
        profile.WaistCm = input.WaistCm;
        profile.HipCm = input.HipCm;
        profile.NeckCm = input.NeckCm;
        profile.TargetWeightKg = input.TargetWeightKg;
        profile.SleepHoursPerNight = input.SleepHoursPerNight;
        profile.Smoking = input.Smoking;
        profile.AlcoholConsumption = input.AlcoholConsumption;
        profile.RestingHeartRate = input.RestingHeartRate;

        await _repository.UpdateAsync(profile);

        var dto = ObjectMapper.Map<UserProfile, UserProfileDto>(profile);
        FillCalculatedFields(dto);
        return dto;
    }

    private static void FillCalculatedFields(UserProfileDto dto)
    {
        if (dto.BirthDate.HasValue)
        {
            var today = DateTime.Today;
            var birth = dto.BirthDate.Value;
            dto.Age = today.Year - birth.Year - (today.DayOfYear < birth.DayOfYear ? 1 : 0);
        }

        if (dto.HeightCm.HasValue && dto.HeightCm > 0 && dto.WeightKg.HasValue && dto.WeightKg > 0)
        {
            var heightM = (double)(dto.HeightCm.Value / 100m);
            dto.Bmi = Math.Round((decimal)((double)dto.WeightKg.Value / (heightM * heightM)), 1);
            dto.IdealWeightMinKg = Math.Round(18.5m * (dto.HeightCm.Value / 100m) * (dto.HeightCm.Value / 100m), 1);
            dto.IdealWeightMaxKg = Math.Round(24.9m * (dto.HeightCm.Value / 100m) * (dto.HeightCm.Value / 100m), 1);
        }

        if (dto.WeightKg.HasValue && dto.WeightKg > 0 && dto.HeightCm.HasValue && dto.HeightCm > 0 && dto.Age.HasValue && dto.Age > 0)
        {
            dto.Bmr = CalculateBmr(dto.Gender, dto.WeightKg.Value, dto.HeightCm.Value, dto.Age.Value);
            var multiplier = GetActivityMultiplier(dto.ActivityLevel);
            dto.Tdee = Math.Round(dto.Bmr!.Value * multiplier, 0);
        }

        if (dto.WaistCm.HasValue && dto.NeckCm.HasValue && dto.HeightCm.HasValue && dto.HeightCm > 0 &&
            (dto.Gender == Gender.Male || (dto.Gender == Gender.Female && dto.HipCm.HasValue)))
        {
            dto.BodyFatPercentage = CalculateBodyFatNavy(dto.Gender, dto.HeightCm.Value, dto.WaistCm.Value, dto.NeckCm.Value, dto.HipCm);
        }
    }

    /// <summary>Mifflin-St Jeor BMR (kcal/gün)</summary>
    private static decimal CalculateBmr(Gender gender, decimal weightKg, decimal heightCm, int age)
    {
        var w = (double)weightKg;
        var h = (double)heightCm;
        double bmr = 10 * w + 6.25 * h - 5 * age;
        bmr += gender == Gender.Male ? 5 : -161;
        return Math.Round((decimal)bmr, 0);
    }

    private static decimal GetActivityMultiplier(ActivityLevel level)
    {
        return level switch
        {
            ActivityLevel.Sedentary => 1.2m,
            ActivityLevel.Light => 1.375m,
            ActivityLevel.Moderate => 1.55m,
            ActivityLevel.Active => 1.725m,
            ActivityLevel.VeryActive => 1.9m,
            _ => 1.2m
        };
    }

    /// <summary>US Navy vücut yağ yüzdesi (erkek: boyun+bel+boy; kadın: boyun+bel+kalça+boy)</summary>
    private static decimal? CalculateBodyFatNavy(Gender gender, decimal heightCm, decimal waistCm, decimal neckCm, decimal? hipCm)
    {
        try
        {
            double h = (double)heightCm;
            double w = (double)waistCm;
            double n = (double)neckCm;
            if (gender == Gender.Male)
            {
                // 495 / (1.0324 - 0.19077*log10(waist-neck) + 0.15456*log10(height)) - 450
                var part = 1.0324 - 0.19077 * Math.Log10((double)waistCm - (double)neckCm) + 0.15456 * Math.Log10(h);
                if (part <= 0) return null;
                var bf = 495 / part - 450;
                return Math.Round((decimal)bf, 1);
            }
            if (gender == Gender.Female && hipCm.HasValue)
            {
                double hip = (double)hipCm.Value;
                var part = 1.29579 - 0.35004 * Math.Log10(w + hip - n) + 0.22100 * Math.Log10(h);
                if (part <= 0) return null;
                var bf = 495 / part - 450;
                return Math.Round((decimal)bf, 1);
            }
        }
        catch
        {
            // log10 negatif veya 0 olabilir
        }
        return null;
    }
}
