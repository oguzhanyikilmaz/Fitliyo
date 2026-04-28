using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
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
public class PersonalWorkoutProgramAppService : FitliyoAppService, IPersonalWorkoutProgramAppService
{
    private readonly IRepository<PersonalWorkoutProgram, Guid> _programRepository;
    private readonly IRepository<PersonalWorkoutTemplateExercise, Guid> _exerciseRepository;

    public PersonalWorkoutProgramAppService(
        IRepository<PersonalWorkoutProgram, Guid> programRepository,
        IRepository<PersonalWorkoutTemplateExercise, Guid> exerciseRepository)
    {
        _programRepository = programRepository;
        _exerciseRepository = exerciseRepository;
    }

    public async Task<PersonalWorkoutProgramDto> GetAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var p = await _programRepository.GetAsync(id);
        if (p.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        return await MapToDtoWithExercisesAsync(p);
    }

    public async Task<PagedResultDto<PersonalWorkoutProgramDto>> GetListAsync(GetPersonalWorkoutProgramListDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var q = await _programRepository.GetQueryableAsync();
        q = q.Where(x => x.UserId == userId);
        if (input.IncludeArchived != true)
            q = q.Where(x => !x.IsArchived);
        var total = await AsyncExecuter.CountAsync(q);
        q = !string.IsNullOrWhiteSpace(input.Sorting) ? q.OrderBy(input.Sorting) : q.OrderByDescending(x => x.CreationTime);
        q = q.PageBy(input);
        var list = await AsyncExecuter.ToListAsync(q);
        if (list.Count == 0)
            return new PagedResultDto<PersonalWorkoutProgramDto>(total, new List<PersonalWorkoutProgramDto>());
        var idList = list.Select(x => x.Id).ToList();
        var allEx = await _exerciseRepository.GetListAsync(x => idList.Contains(x.PersonalWorkoutProgramId));
        var exMap = allEx.GroupBy(x => x.PersonalWorkoutProgramId)
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.DayNumber).ThenBy(x => x.SortOrder).ToList());
        var result = new List<PersonalWorkoutProgramDto>();
        foreach (var p in list)
        {
            var dto = ObjectMapper.Map<PersonalWorkoutProgram, PersonalWorkoutProgramDto>(p);
            exMap.TryGetValue(p.Id, out var exs);
            dto.Exercises = exs == null
                ? new List<PersonalWorkoutTemplateExerciseDto>()
                : ObjectMapper.Map<List<PersonalWorkoutTemplateExercise>, List<PersonalWorkoutTemplateExerciseDto>>(exs);
            result.Add(dto);
        }
        return new PagedResultDto<PersonalWorkoutProgramDto>(total, result);
    }

    public async Task<PersonalWorkoutProgramDto> CreateAsync(CreateUpdatePersonalWorkoutProgramDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var p = new PersonalWorkoutProgram(GuidGenerator.Create(), userId, input.Title)
        {
            Description = input.Description,
            WeekdayIndex = input.WeekdayIndex,
            IsArchived = input.IsArchived
        };
        await _programRepository.InsertAsync(p, autoSave: true);
        await SyncExercisesAsync(p.Id, input.Exercises);
        Logger.LogInformation("Kişisel antrenman programı eklendi: {Id}, {UserId}", p.Id, userId);
        return await MapToDtoWithExercisesAsync(p);
    }

    public async Task<PersonalWorkoutProgramDto> UpdateAsync(Guid id, CreateUpdatePersonalWorkoutProgramDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var p = await _programRepository.GetAsync(id);
        if (p.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        p.Title = input.Title;
        p.Description = input.Description;
        p.WeekdayIndex = input.WeekdayIndex;
        p.IsArchived = input.IsArchived;
        await _programRepository.UpdateAsync(p, autoSave: true);
        await RemoveExercisesByProgramIdAsync(p.Id);
        await SyncExercisesAsync(p.Id, input.Exercises);
        return await MapToDtoWithExercisesAsync(p);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var p = await _programRepository.GetAsync(id);
        if (p.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        await RemoveExercisesByProgramIdAsync(p.Id);
        await _programRepository.DeleteAsync(p);
    }

    private async Task RemoveExercisesByProgramIdAsync(Guid programId)
    {
        var all = await _exerciseRepository.GetListAsync(x => x.PersonalWorkoutProgramId == programId);
        foreach (var e in all)
            await _exerciseRepository.DeleteAsync(e, autoSave: true);
    }

    private async Task SyncExercisesAsync(Guid programId, List<CreateUpdatePersonalWorkoutTemplateExerciseDto> items)
    {
        var order = 0;
        foreach (var s in items)
        {
            var e = new PersonalWorkoutTemplateExercise(GuidGenerator.Create())
            {
                PersonalWorkoutProgramId = programId,
                DayNumber = s.DayNumber,
                SortOrder = s.SortOrder > 0 ? s.SortOrder : order,
                Name = s.Name,
                TargetSets = s.TargetSets,
                TargetReps = s.TargetReps,
                SuggestedDurationMinutes = s.SuggestedDurationMinutes,
                DefaultMet = s.DefaultMet,
                Notes = s.Notes
            };
            await _exerciseRepository.InsertAsync(e, autoSave: true);
            order++;
        }
    }

    private async Task<PersonalWorkoutProgramDto> MapToDtoWithExercisesAsync(PersonalWorkoutProgram p)
    {
        var list = await _exerciseRepository.GetListAsync(x => x.PersonalWorkoutProgramId == p.Id);
        var sorted = list.OrderBy(x => x.DayNumber).ThenBy(x => x.SortOrder).ToList();
        var dto = ObjectMapper.Map<PersonalWorkoutProgram, PersonalWorkoutProgramDto>(p);
        dto.Exercises = ObjectMapper.Map<List<PersonalWorkoutTemplateExercise>, List<PersonalWorkoutTemplateExerciseDto>>(sorted);
        return dto;
    }
}
