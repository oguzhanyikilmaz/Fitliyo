using System;
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
public class PersonalNutritionPlanAppService : FitliyoAppService, IPersonalNutritionPlanAppService
{
    private readonly IRepository<PersonalNutritionPlan, Guid> _repository;

    public PersonalNutritionPlanAppService(IRepository<PersonalNutritionPlan, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<PersonalNutritionPlanDto> GetAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var e = await _repository.GetAsync(id);
        if (e.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        return ObjectMapper.Map<PersonalNutritionPlan, PersonalNutritionPlanDto>(e);
    }

    public async Task<PagedResultDto<PersonalNutritionPlanDto>> GetListAsync(GetPersonalNutritionPlanListDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var q = await _repository.GetQueryableAsync();
        q = q.Where(x => x.UserId == userId);
        if (input.ActiveOnly == true)
            q = q.Where(x => x.IsActive);

        var total = await AsyncExecuter.CountAsync(q);
        q = !string.IsNullOrWhiteSpace(input.Sorting) ? q.OrderBy(input.Sorting) : q.OrderByDescending(x => x.CreationTime);
        q = q.PageBy(input);
        var list = await AsyncExecuter.ToListAsync(q);
        return new PagedResultDto<PersonalNutritionPlanDto>(total, ObjectMapper.Map<System.Collections.Generic.List<PersonalNutritionPlan>, System.Collections.Generic.List<PersonalNutritionPlanDto>>(list));
    }

    public async Task<PersonalNutritionPlanDto> CreateAsync(CreateUpdatePersonalNutritionPlanDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var e = new PersonalNutritionPlan(GuidGenerator.Create(), userId, input.Title)
        {
            Description = input.Description,
            DailyCalorieTarget = input.DailyCalorieTarget,
            DailyProteinTargetG = input.DailyProteinTargetG,
            DailyCarbsTargetG = input.DailyCarbsTargetG,
            DailyFatTargetG = input.DailyFatTargetG,
            IsActive = input.IsActive
        };
        if (e.IsActive)
            await DeactivateOtherPlansAsync(userId, e.Id);
        await _repository.InsertAsync(e, autoSave: true);
        Logger.LogInformation("Beslenme planı oluşturuldu: {Id}, {UserId}", e.Id, userId);
        return ObjectMapper.Map<PersonalNutritionPlan, PersonalNutritionPlanDto>(e);
    }

    public async Task<PersonalNutritionPlanDto> UpdateAsync(Guid id, CreateUpdatePersonalNutritionPlanDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var e = await _repository.GetAsync(id);
        if (e.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        e.Title = input.Title;
        e.Description = input.Description;
        e.DailyCalorieTarget = input.DailyCalorieTarget;
        e.DailyProteinTargetG = input.DailyProteinTargetG;
        e.DailyCarbsTargetG = input.DailyCarbsTargetG;
        e.DailyFatTargetG = input.DailyFatTargetG;
        e.IsActive = input.IsActive;
        if (e.IsActive)
            await DeactivateOtherPlansAsync(userId, e.Id);
        await _repository.UpdateAsync(e);
        return ObjectMapper.Map<PersonalNutritionPlan, PersonalNutritionPlanDto>(e);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var e = await _repository.GetAsync(id);
        if (e.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        await _repository.DeleteAsync(e);
    }

    private async Task DeactivateOtherPlansAsync(Guid userId, Guid keepId)
    {
        var all = await _repository.GetListAsync(x => x.UserId == userId && x.IsActive && x.Id != keepId);
        foreach (var p in all)
        {
            p.IsActive = false;
            await _repository.UpdateAsync(p, autoSave: true);
        }
    }
}
