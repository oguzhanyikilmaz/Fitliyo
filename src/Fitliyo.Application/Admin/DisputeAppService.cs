using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Admin.Dtos;
using Fitliyo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Admin;

[Authorize]
public class DisputeAppService : FitliyoAppService, IDisputeAppService
{
    private readonly IRepository<Dispute, Guid> _repository;

    public DisputeAppService(IRepository<Dispute, Guid> repository)
    {
        _repository = repository;
    }

    [Authorize]
    public async Task<DisputeDto> CreateAsync(CreateDisputeDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var entity = new Dispute(
            GuidGenerator.Create(),
            input.OrderId,
            userId,
            input.DisputeType,
            input.Description);
        await _repository.InsertAsync(entity);
        return ObjectMapper.Map<Dispute, DisputeDto>(entity);
    }

    [Authorize]
    public async Task<DisputeDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<Dispute, DisputeDto>(entity);
    }

    [Authorize]
    public async Task<PagedResultDto<DisputeDto>> GetMyDisputesAsync(GetDisputeListDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var queryable = await _repository.GetQueryableAsync();
        queryable = queryable.Where(x => x.OpenedByUserId == userId);
        if (input.Status.HasValue) queryable = queryable.Where(x => x.Status == input.Status.Value);
        if (input.OrderId.HasValue) queryable = queryable.Where(x => x.OrderId == input.OrderId.Value);
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<DisputeDto>(totalCount, items.Select(x => ObjectMapper.Map<Dispute, DisputeDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Admin.Disputes)]
    public async Task<PagedResultDto<DisputeDto>> GetListAsync(GetDisputeListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        if (input.Status.HasValue) queryable = queryable.Where(x => x.Status == input.Status.Value);
        if (input.OrderId.HasValue) queryable = queryable.Where(x => x.OrderId == input.OrderId.Value);
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<DisputeDto>(totalCount, items.Select(x => ObjectMapper.Map<Dispute, DisputeDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Admin.Disputes)]
    public async Task<DisputeDto> ResolveAsync(Guid id, ResolveDisputeDto input)
    {
        var entity = await _repository.GetAsync(id);
        var userId = (CurrentUser.Id ?? Guid.Empty);
        entity.Resolve(input.ResolutionNote, userId);
        await _repository.UpdateAsync(entity);
        return ObjectMapper.Map<Dispute, DisputeDto>(entity);
    }
}
