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
public class UserFoodItemAppService : FitliyoAppService, IUserFoodItemAppService
{
    private readonly IRepository<UserFoodItem, Guid> _repository;

    public UserFoodItemAppService(IRepository<UserFoodItem, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UserFoodItemDto> GetAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var entity = await _repository.GetAsync(id);
        if (entity.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        return ObjectMapper.Map<UserFoodItem, UserFoodItemDto>(entity);
    }

    public async Task<PagedResultDto<UserFoodItemDto>> GetListAsync(GetUserFoodItemListDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var q = await _repository.GetQueryableAsync();
        q = q.Where(x => x.UserId == userId);
        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            var f = input.Filter.Trim();
            q = q.Where(x => x.Name.Contains(f));
        }

        var total = await AsyncExecuter.CountAsync(q);
        q = !string.IsNullOrWhiteSpace(input.Sorting) ? q.OrderBy(input.Sorting) : q.OrderBy(x => x.Name);
        q = q.PageBy(input);
        var list = await AsyncExecuter.ToListAsync(q);
        return new PagedResultDto<UserFoodItemDto>(total, ObjectMapper.Map<System.Collections.Generic.List<UserFoodItem>, System.Collections.Generic.List<UserFoodItemDto>>(list));
    }

    public async Task<UserFoodItemDto> CreateAsync(CreateUpdateUserFoodItemDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var e = new UserFoodItem(GuidGenerator.Create(), userId, input.Name)
        {
            KcalPer100G = input.KcalPer100G,
            ProteinPer100G = input.ProteinPer100G,
            CarbsPer100G = input.CarbsPer100G,
            FatPer100G = input.FatPer100G
        };
        await _repository.InsertAsync(e, autoSave: true);
        Logger.LogInformation("Kullanıcı gıdası eklendi: {Id}, {UserId}", e.Id, userId);
        return ObjectMapper.Map<UserFoodItem, UserFoodItemDto>(e);
    }

    public async Task<UserFoodItemDto> UpdateAsync(Guid id, CreateUpdateUserFoodItemDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var e = await _repository.GetAsync(id);
        if (e.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        e.Name = input.Name;
        e.KcalPer100G = input.KcalPer100G;
        e.ProteinPer100G = input.ProteinPer100G;
        e.CarbsPer100G = input.CarbsPer100G;
        e.FatPer100G = input.FatPer100G;
        await _repository.UpdateAsync(e);
        return ObjectMapper.Map<UserFoodItem, UserFoodItemDto>(e);
    }

    public async Task DeleteAsync(Guid id)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var e = await _repository.GetAsync(id);
        if (e.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
        await _repository.DeleteAsync(e);
    }
}
