using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo;
using Fitliyo.Wellness.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Wellness;

[Authorize]
public class UserMealLogAppService : FitliyoAppService, IUserMealLogAppService
{
    private readonly IRepository<UserDailyMealLog, Guid> _dailyRepository;
    private readonly IRepository<UserMealLogEntry, Guid> _entryRepository;
    private readonly IRepository<UserFoodItem, Guid> _foodRepository;

    public UserMealLogAppService(
        IRepository<UserDailyMealLog, Guid> dailyRepository,
        IRepository<UserMealLogEntry, Guid> entryRepository,
        IRepository<UserFoodItem, Guid> foodRepository)
    {
        _dailyRepository = dailyRepository;
        _entryRepository = entryRepository;
        _foodRepository = foodRepository;
    }

    public async Task<UserDailyMealLogDto> GetOrCreateDailyLogAsync(DateTime logDate)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var day = logDate.Date;
        var log = await _dailyRepository.FindAsync(x => x.UserId == userId && x.LogDate == day);
        if (log == null)
        {
            log = new UserDailyMealLog(GuidGenerator.Create(), userId, day);
            await _dailyRepository.InsertAsync(log, autoSave: true);
        }
        return await MapDailyLogAsync(log);
    }

    public async Task<UserMealLogEntryDto> AddEntryAsync(Guid dailyLogId, CreateMealLogEntryDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var dayLog = await _dailyRepository.GetAsync(dailyLogId);
        if (dayLog.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        return await AddEntryCoreAsync(dayLog, input);
    }

    public async Task<UserMealLogEntryDto> UpdateEntryAsync(Guid entryId, UpdateMealLogEntryDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var entry = await _entryRepository.GetAsync(entryId);
        var dayLog = await _dailyRepository.GetAsync(entry.UserDailyMealLogId);
        if (dayLog.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        await FillEntryFromInputAsync(userId, entry, input);
        await _entryRepository.UpdateAsync(entry, autoSave: true);
        return ObjectMapper.Map<UserMealLogEntry, UserMealLogEntryDto>(entry);
    }

    public async Task RemoveEntryAsync(Guid entryId)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var entry = await _entryRepository.GetAsync(entryId);
        var dayLog = await _dailyRepository.GetAsync(entry.UserDailyMealLogId);
        if (dayLog.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        await _entryRepository.DeleteAsync(entry);
        Logger.LogInformation("Öğün satırı silindi: {EntryId}, {UserId}", entryId, userId);
    }

    private async Task<UserMealLogEntryDto> AddEntryCoreAsync(UserDailyMealLog dayLog, CreateMealLogEntryDto input)
    {
        var userId = dayLog.UserId;
        var entry = new UserMealLogEntry(GuidGenerator.Create())
        {
            UserDailyMealLogId = dayLog.Id,
            MealType = input.MealType
        };
        var updateDto = new UpdateMealLogEntryDto
        {
            MealType = input.MealType,
            UserFoodItemId = input.UserFoodItemId,
            FoodName = input.FoodName,
            PortionGrams = input.PortionGrams,
            KcalPer100GManual = input.KcalPer100GManual,
            ProteinPer100GManual = input.ProteinPer100GManual,
            CarbsPer100GManual = input.CarbsPer100GManual,
            FatPer100GManual = input.FatPer100GManual
        };
        await FillEntryFromInputAsync(userId, entry, updateDto);
        await _entryRepository.InsertAsync(entry, autoSave: true);
        Logger.LogInformation("Öğün eklendi: {EntryId}, {UserId}", entry.Id, userId);
        return ObjectMapper.Map<UserMealLogEntry, UserMealLogEntryDto>(entry);
    }

    private async Task FillEntryFromInputAsync(
        Guid userId,
        UserMealLogEntry entry,
        UpdateMealLogEntryDto input)
    {
        entry.MealType = input.MealType;
        entry.UserFoodItemId = input.UserFoodItemId;
        entry.FoodName = input.FoodName;
        entry.PortionGrams = input.PortionGrams;
        if (input.UserFoodItemId.HasValue)
        {
            var food = await _foodRepository.GetAsync(input.UserFoodItemId.Value);
            if (food.UserId != userId)
                throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
            entry.FoodName = food.Name;
            WellnessMetabolicMath.CalculateKcalAndMacrosForPortion(
                food.KcalPer100G,
                food.ProteinPer100G,
                food.CarbsPer100G,
                food.FatPer100G,
                input.PortionGrams,
                out var kcal,
                out var p,
                out var c,
                out var f);
            entry.Calories = kcal;
            entry.ProteinG = p;
            entry.CarbsG = c;
            entry.FatG = f;
            return;
        }
        if (string.IsNullOrWhiteSpace(input.FoodName) || !input.KcalPer100GManual.HasValue)
            throw new BusinessException(FitliyoDomainErrorCodes.MealLogManualInputInvalid);
        var p100 = input.ProteinPer100GManual ?? 0;
        var c100 = input.CarbsPer100GManual ?? 0;
        var f100 = input.FatPer100GManual ?? 0;
        WellnessMetabolicMath.CalculateKcalAndMacrosForPortion(
            input.KcalPer100GManual.Value,
            p100,
            c100,
            f100,
            input.PortionGrams,
            out var kcal2,
            out var p2,
            out var c2,
            out var f2);
        entry.Calories = kcal2;
        entry.ProteinG = p2;
        entry.CarbsG = c2;
        entry.FatG = f2;
    }

    private async Task<UserDailyMealLogDto> MapDailyLogAsync(UserDailyMealLog log)
    {
        var entries = await _entryRepository.GetListAsync(x => x.UserDailyMealLogId == log.Id);
        var dto = ObjectMapper.Map<UserDailyMealLog, UserDailyMealLogDto>(log);
        var list = entries.ToList();
        dto.Entries = ObjectMapper.Map<List<UserMealLogEntry>, List<UserMealLogEntryDto>>(list);
        dto.TotalCalories = list.Sum(x => x.Calories);
        dto.TotalProteinG = list.Sum(x => x.ProteinG);
        dto.TotalCarbsG = list.Sum(x => x.CarbsG);
        dto.TotalFatG = list.Sum(x => x.FatG);
        return dto;
    }
}
