using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Permissions;
using Fitliyo.Support.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Support;

[Authorize]
public class SupportTicketAppService : FitliyoAppService, ISupportTicketAppService
{
    private readonly IRepository<SupportTicket, Guid> _repository;

    public SupportTicketAppService(IRepository<SupportTicket, Guid> repository)
    {
        _repository = repository;
    }

    [Authorize(FitliyoPermissions.Support.Default)]
    public async Task<SupportTicketDto> CreateAsync(CreateSupportTicketDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var entity = new SupportTicket(
            GuidGenerator.Create(),
            input.Subject,
            input.Message,
            input.Category,
            userId,
            input.OrderId);
        await _repository.InsertAsync(entity);
        return ObjectMapper.Map<SupportTicket, SupportTicketDto>(entity);
    }

    [Authorize(FitliyoPermissions.Support.Default)]
    public async Task<SupportTicketDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        var userId = (CurrentUser.Id ?? Guid.Empty);
        if (entity.UserId != userId)
            await AuthorizationService.CheckAsync(FitliyoPermissions.Support.Manage);
        return ObjectMapper.Map<SupportTicket, SupportTicketDto>(entity);
    }

    [Authorize(FitliyoPermissions.Support.Default)]
    public async Task<PagedResultDto<SupportTicketDto>> GetMyTicketsAsync(GetSupportTicketListDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var queryable = await _repository.GetQueryableAsync();
        queryable = queryable.Where(x => x.UserId == userId);
        if (input.Status.HasValue) queryable = queryable.Where(x => x.Status == input.Status.Value);
        if (input.Category.HasValue) queryable = queryable.Where(x => x.Category == input.Category.Value);
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<SupportTicketDto>(totalCount, items.Select(x => ObjectMapper.Map<SupportTicket, SupportTicketDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Support.Manage)]
    public async Task<PagedResultDto<SupportTicketDto>> GetListAsync(GetSupportTicketListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        if (input.Status.HasValue) queryable = queryable.Where(x => x.Status == input.Status.Value);
        if (input.Category.HasValue) queryable = queryable.Where(x => x.Category == input.Category.Value);
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<SupportTicketDto>(totalCount, items.Select(x => ObjectMapper.Map<SupportTicket, SupportTicketDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Support.Manage)]
    public async Task<SupportTicketDto> ReplyAsync(Guid id, ReplySupportTicketDto input)
    {
        var entity = await _repository.GetAsync(id);
        entity.SetAdminReply(input.AdminReply);
        await _repository.UpdateAsync(entity);
        return ObjectMapper.Map<SupportTicket, SupportTicketDto>(entity);
    }

    [Authorize(FitliyoPermissions.Support.Manage)]
    public async Task<SupportTicketDto> UpdateStatusAsync(Guid id, SupportTicketStatus status)
    {
        var entity = await _repository.GetAsync(id);
        entity.Status = status;
        await _repository.UpdateAsync(entity);
        return ObjectMapper.Map<SupportTicket, SupportTicketDto>(entity);
    }
}
