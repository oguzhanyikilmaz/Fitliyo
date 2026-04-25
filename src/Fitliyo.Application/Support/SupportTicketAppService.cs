using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Permissions;
using Fitliyo.Support.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Fitliyo.Support;

[Authorize]
public class SupportTicketAppService : FitliyoAppService, ISupportTicketAppService
{
    private readonly IRepository<SupportTicket, Guid> _repository;
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

    public SupportTicketAppService(
        IRepository<SupportTicket, Guid> repository,
        IRepository<IdentityUser, Guid> identityUserRepository)
    {
        _repository = repository;
        _identityUserRepository = identityUserRepository;
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
        await PopulateTicketDisplayNameAsync(entity);
        await _repository.InsertAsync(entity);
        return MapTicketToDto(entity);
    }

    [Authorize(FitliyoPermissions.Support.Default)]
    public async Task<SupportTicketDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        var userId = (CurrentUser.Id ?? Guid.Empty);
        if (entity.UserId != userId)
            await AuthorizationService.CheckAsync(FitliyoPermissions.Support.Manage);
        await EnrichTicketDisplayNamesAsync([entity]);
        return MapTicketToDto(entity);
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
        await EnrichTicketDisplayNamesAsync(items);
        return new PagedResultDto<SupportTicketDto>(totalCount, items.Select(MapTicketToDto).ToList());
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
        await EnrichTicketDisplayNamesAsync(items);
        return new PagedResultDto<SupportTicketDto>(totalCount, items.Select(MapTicketToDto).ToList());
    }

    [Authorize(FitliyoPermissions.Support.Manage)]
    public async Task<SupportTicketDto> ReplyAsync(Guid id, ReplySupportTicketDto input)
    {
        var entity = await _repository.GetAsync(id);
        entity.SetAdminReply(input.AdminReply);
        await _repository.UpdateAsync(entity);
        await EnrichTicketDisplayNamesAsync([entity]);
        return MapTicketToDto(entity);
    }

    [Authorize(FitliyoPermissions.Support.Manage)]
    public async Task<SupportTicketDto> UpdateStatusAsync(Guid id, SupportTicketStatus status)
    {
        var entity = await _repository.GetAsync(id);
        entity.Status = status;
        await _repository.UpdateAsync(entity);
        await EnrichTicketDisplayNamesAsync([entity]);
        return MapTicketToDto(entity);
    }

    private async Task PopulateTicketDisplayNameAsync(SupportTicket ticket)
    {
        if (!ticket.UserId.HasValue)
        {
            return;
        }

        var user = await _identityUserRepository.FindAsync(ticket.UserId.Value);
        var fullName = BuildFullName(user?.Name, user?.Surname);
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            ticket.SetProperty("UserFullName", fullName);
        }
    }

    private async Task EnrichTicketDisplayNamesAsync(IReadOnlyList<SupportTicket> tickets)
    {
        if (tickets.Count == 0)
        {
            return;
        }

        var userIds = tickets
            .Where(x => x.UserId.HasValue)
            .Select(x => x.UserId!.Value)
            .Distinct()
            .ToList();

        if (userIds.Count == 0)
        {
            return;
        }

        var usersQuery = await _identityUserRepository.GetQueryableAsync();
        var users = await AsyncExecuter.ToListAsync(usersQuery.Where(x => userIds.Contains(x.Id)));
        var userNameMap = users.ToDictionary(x => x.Id, x => BuildFullName(x.Name, x.Surname));

        var dirtyTickets = new List<SupportTicket>();
        foreach (var ticket in tickets)
        {
            if (!ticket.UserId.HasValue)
            {
                continue;
            }

            var existingName = ticket.GetProperty<string>("UserFullName");
            if (!string.IsNullOrWhiteSpace(existingName))
            {
                continue;
            }

            if (userNameMap.TryGetValue(ticket.UserId.Value, out var resolvedName) && !string.IsNullOrWhiteSpace(resolvedName))
            {
                ticket.SetProperty("UserFullName", resolvedName);
                dirtyTickets.Add(ticket);
            }
        }

        if (dirtyTickets.Count > 0)
        {
            await _repository.UpdateManyAsync(dirtyTickets);
        }
    }

    private SupportTicketDto MapTicketToDto(SupportTicket ticket)
    {
        var dto = ObjectMapper.Map<SupportTicket, SupportTicketDto>(ticket);
        dto.UserFullName = ticket.GetProperty<string>("UserFullName");
        return dto;
    }

    private static string? BuildFullName(string? name, string? surname)
    {
        var fullName = $"{name} {surname}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
    }
}
