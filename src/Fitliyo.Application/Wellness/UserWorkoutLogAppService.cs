using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Profiles;
using Fitliyo;
using Fitliyo.Wellness.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Wellness;

[Authorize]
public class UserWorkoutLogAppService : FitliyoAppService, IUserWorkoutLogAppService
{
    private readonly IRepository<UserWorkoutLog, Guid> _logRepository;
    private readonly IRepository<UserWorkoutLogLine, Guid> _lineRepository;
    private readonly IRepository<PersonalWorkoutProgram, Guid> _programRepository;
    private readonly IRepository<UserProfile, Guid> _profileRepository;

    public UserWorkoutLogAppService(
        IRepository<UserWorkoutLog, Guid> logRepository,
        IRepository<UserWorkoutLogLine, Guid> lineRepository,
        IRepository<PersonalWorkoutProgram, Guid> programRepository,
        IRepository<UserProfile, Guid> profileRepository)
    {
        _logRepository = logRepository;
        _lineRepository = lineRepository;
        _programRepository = programRepository;
        _profileRepository = profileRepository;
    }

    public async Task<UserWorkoutLogDto> GetAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var log = await _logRepository.GetAsync(id);
        if (log.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        return await MapLogAsync(log);
    }

    public async Task<PagedResultDto<UserWorkoutLogDto>> GetListAsync(GetUserWorkoutLogListDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var q = await _logRepository.GetQueryableAsync();
        q = q.Where(x => x.UserId == userId);
        if (input.FromDate.HasValue)
            q = q.Where(x => x.LogDate >= input.FromDate.Value.Date);
        if (input.ToDate.HasValue)
            q = q.Where(x => x.LogDate <= input.ToDate.Value.Date);
        var total = await AsyncExecuter.CountAsync(q);
        q = !string.IsNullOrWhiteSpace(input.Sorting) ? q.OrderBy(input.Sorting) : q.OrderByDescending(x => x.LogDate);
        q = q.PageBy(input);
        var list = await AsyncExecuter.ToListAsync(q);
        if (list.Count == 0)
            return new PagedResultDto<UserWorkoutLogDto>(0, new List<UserWorkoutLogDto>());
        var logIds = list.Select(x => x.Id).ToList();
        var allLines = await _lineRepository.GetListAsync(x => logIds.Contains(x.UserWorkoutLogId));
        var lineMap = allLines.GroupBy(x => x.UserWorkoutLogId)
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.ExerciseName).ToList());
        var result = new List<UserWorkoutLogDto>();
        foreach (var log in list)
        {
            lineMap.TryGetValue(log.Id, out var lines);
            result.Add(ObjectMapperMapLog(log, lines));
        }
        return new PagedResultDto<UserWorkoutLogDto>(total, result);
    }

    public async Task<UserWorkoutLogDto> CreateAsync(CreateUserWorkoutLogDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        if (input.PersonalWorkoutProgramId.HasValue)
        {
            var program = await _programRepository.GetAsync(input.PersonalWorkoutProgramId.Value);
            if (program.UserId != userId)
                throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        }

        var weightKg = await GetUserWeightKgOrThrowAsync(userId);
        var log = new UserWorkoutLog(GuidGenerator.Create(), userId, input.LogDate)
        {
            PersonalWorkoutProgramId = input.PersonalWorkoutProgramId,
            Notes = input.Notes
        };
        decimal totalBurn = 0;
        var lines = new List<UserWorkoutLogLine>();
        foreach (var s in input.Lines)
        {
            var kcal = WellnessMetabolicMath.CalculateKcalFromMet(s.Met, weightKg, s.DurationMinutes);
            totalBurn += kcal;
            var line = new UserWorkoutLogLine(GuidGenerator.Create())
            {
                UserWorkoutLogId = log.Id,
                ExerciseName = s.ExerciseName,
                DurationMinutes = s.DurationMinutes,
                Met = s.Met,
                CaloriesBurned = kcal
            };
            lines.Add(line);
        }
        log.TotalCaloriesBurned = totalBurn;
        await _logRepository.InsertAsync(log, autoSave: true);
        foreach (var line in lines)
            await _lineRepository.InsertAsync(line, autoSave: true);
        Logger.LogInformation("Antrenman günlüğü: {Id}, {UserId}, {Kcal}", log.Id, userId, totalBurn);
        return await MapLogAsync(log);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var log = await _logRepository.GetAsync(id);
        if (log.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        var lines = await _lineRepository.GetListAsync(x => x.UserWorkoutLogId == id);
        foreach (var l in lines)
            await _lineRepository.DeleteAsync(l, autoSave: true);
        await _logRepository.DeleteAsync(log);
    }

    private async Task<decimal> GetUserWeightKgOrThrowAsync(Guid userId)
    {
        var pro = await _profileRepository.FindAsync(x => x.UserId == userId);
        if (pro?.WeightKg is not > 0)
            throw new BusinessException(FitliyoDomainErrorCodes.UserWeightRequiredForWorkoutKcal);
        return pro.WeightKg!.Value;
    }

    private async Task<UserWorkoutLogDto> MapLogAsync(UserWorkoutLog log)
    {
        var lines = await _lineRepository.GetListAsync(x => x.UserWorkoutLogId == log.Id);
        return ObjectMapperMapLog(log, lines.OrderBy(x => x.ExerciseName).ToList());
    }

    private UserWorkoutLogDto ObjectMapperMapLog(UserWorkoutLog log, List<UserWorkoutLogLine>? lines)
    {
        var dto = ObjectMapper.Map<UserWorkoutLog, UserWorkoutLogDto>(log);
        dto.Lines = lines == null
            ? new List<UserWorkoutLogLineDto>()
            : ObjectMapper.Map<List<UserWorkoutLogLine>, List<UserWorkoutLogLineDto>>(lines);
        return dto;
    }
}
