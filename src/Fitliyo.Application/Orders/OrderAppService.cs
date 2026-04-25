using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Orders.Dtos;
using Fitliyo.ServicePackages;
using Fitliyo.Permissions;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Fitliyo.Orders;

[Authorize]
public class OrderAppService : FitliyoAppService, IOrderAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<Session, Guid> _sessionRepository;
    private readonly IRepository<ServicePackage, Guid> _packageRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

    public OrderAppService(
        IRepository<Order, Guid> orderRepository,
        IRepository<Session, Guid> sessionRepository,
        IRepository<ServicePackage, Guid> packageRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<IdentityUser, Guid> identityUserRepository)
    {
        _orderRepository = orderRepository;
        _sessionRepository = sessionRepository;
        _packageRepository = packageRepository;
        _trainerProfileRepository = trainerProfileRepository;
        _identityUserRepository = identityUserRepository;
    }

    [Authorize]
    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        var userId = (CurrentUser.Id ?? Guid.Empty);

        if (order.StudentId != userId)
        {
            var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
            if (trainerProfile == null || trainerProfile.Id != order.TrainerProfileId)
            {
                await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);
            }
        }

        await EnrichOrderDisplayNamesAsync([order]);

        var dto = ObjectMapper.Map<Order, OrderDto>(order);
        dto.StudentFullName = order.GetProperty<string>("StudentFullName");
        dto.TrainerFullName = order.GetProperty<string>("TrainerFullName");
        var package = await _packageRepository.GetAsync(order.ServicePackageId);
        dto.PackageSessionCount = package.SessionCount;
        dto.PackageDurationDays = package.DurationDays;
        return dto;
    }

    [Authorize]
    public async Task<PagedResultDto<OrderDto>> GetMyOrdersAsync(GetOrderListDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var queryable = await _orderRepository.GetQueryableAsync();

        queryable = queryable.Where(x => x.StudentId == userId);

        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status.Value);

        if (input.PaymentStatus.HasValue)
            queryable = queryable.Where(x => x.PaymentStatus == input.PaymentStatus.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = !string.IsNullOrWhiteSpace(input.Sorting)
            ? queryable.OrderBy(input.Sorting)
            : queryable.OrderByDescending(x => x.CreationTime);

        queryable = queryable.PageBy(input);
        var entities = await AsyncExecuter.ToListAsync(queryable);
        await EnrichOrderDisplayNamesAsync(entities);
        var dtos = entities.Select(MapOrderToDto).ToList();
        return new PagedResultDto<OrderDto>(totalCount, dtos);
    }

    [Authorize]
    public async Task<PagedResultDto<OrderDto>> GetTrainerOrdersAsync(GetOrderListDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (trainerProfile == null)
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);

        var queryable = await _orderRepository.GetQueryableAsync();
        queryable = queryable.Where(x => x.TrainerProfileId == trainerProfile.Id);

        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = !string.IsNullOrWhiteSpace(input.Sorting)
            ? queryable.OrderBy(input.Sorting)
            : queryable.OrderByDescending(x => x.CreationTime);

        queryable = queryable.PageBy(input);
        var entities = await AsyncExecuter.ToListAsync(queryable);
        await EnrichOrderDisplayNamesAsync(entities);
        var dtos = entities.Select(MapOrderToDto).ToList();
        return new PagedResultDto<OrderDto>(totalCount, dtos);
    }

    [Authorize(FitliyoPermissions.Admin.Dashboard)]
    public async Task<PagedResultDto<OrderDto>> GetListAsync(GetOrderListDto input)
    {
        var queryable = await _orderRepository.GetQueryableAsync();

        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status.Value);

        if (input.PaymentStatus.HasValue)
            queryable = queryable.Where(x => x.PaymentStatus == input.PaymentStatus.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = !string.IsNullOrWhiteSpace(input.Sorting)
            ? queryable.OrderBy(input.Sorting)
            : queryable.OrderByDescending(x => x.CreationTime);

        queryable = queryable.PageBy(input);
        var entities = await AsyncExecuter.ToListAsync(queryable);
        await EnrichOrderDisplayNamesAsync(entities);
        var dtos = entities.Select(MapOrderToDto).ToList();
        return new PagedResultDto<OrderDto>(totalCount, dtos);
    }

    [Authorize]
    public async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var package = await _packageRepository.GetAsync(input.ServicePackageId);
        var trainerProfile = await _trainerProfileRepository.GetAsync(package.TrainerProfileId);

        if (trainerProfile.UserId == userId)
            throw new BusinessException(FitliyoDomainErrorCodes.CannotPurchaseOwnPackage);

        var orderNumber = GenerateOrderNumber();
        var effectivePrice = package.DiscountedPrice ?? package.Price;

        var order = new Order(
            GuidGenerator.Create(),
            orderNumber,
            userId,
            trainerProfile.Id,
            package.Id,
            effectivePrice,
            input.Quantity);

        order.Notes = input.Notes;
        await PopulateOrderDisplayNamesAsync(order, trainerProfile);

        await _orderRepository.InsertAsync(order);
        Logger.LogInformation("Sipariş oluşturuldu: {OrderId}, {OrderNumber}, Öğrenci: {StudentId}", order.Id, orderNumber, userId);

        return MapOrderToDto(order);
    }

    [Authorize]
    public async Task<OrderDto> CancelAsync(Guid id, string? reason)
    {
        var order = await _orderRepository.GetAsync(id);
        var userId = (CurrentUser.Id ?? Guid.Empty);

        if (order.StudentId != userId)
            await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);

        if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.Refunded)
            throw new BusinessException(FitliyoDomainErrorCodes.OrderCannotBeCancelled);

        order.Cancel(reason);

        if (order.PaymentStatus == PaymentStatus.Escrow)
            order.PaymentStatus = PaymentStatus.Refunded;

        await _orderRepository.UpdateAsync(order);
        Logger.LogInformation("Sipariş iptal edildi: {OrderId}, Sebep: {Reason}", id, reason);

        return MapOrderToDto(order);
    }

    [Authorize]
    public async Task<OrderDto> CompleteAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        var userId = (CurrentUser.Id ?? Guid.Empty);

        var trainerProfile = await _trainerProfileRepository.GetAsync(order.TrainerProfileId);
        if (trainerProfile.UserId != userId)
            await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);

        order.Complete();
        await _orderRepository.UpdateAsync(order);

        trainerProfile.TotalStudentCount++;
        await _trainerProfileRepository.UpdateAsync(trainerProfile);

        Logger.LogInformation("Sipariş tamamlandı: {OrderId}", id);

        return MapOrderToDto(order);
    }

    [Authorize]
    public async Task<PagedResultDto<SessionDto>> GetSessionsAsync(Guid orderId)
    {
        var order = await _orderRepository.GetAsync(orderId);
        var userId = (CurrentUser.Id ?? Guid.Empty);

        if (order.StudentId != userId)
        {
            var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
            if (trainerProfile == null || trainerProfile.Id != order.TrainerProfileId)
                await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);
        }

        var sessions = await _sessionRepository.GetListAsync(x => x.OrderId == orderId);
        var sorted = sessions.OrderBy(x => x.SequenceNumber).ToList();

        return new PagedResultDto<SessionDto>(sorted.Count, sorted.Select(x => ObjectMapper.Map<Session, SessionDto>(x)).ToList());
    }

    [Authorize]
    public async Task<OrderDto> UpdateStudentFormAsync(Guid orderId, UpdateOrderStudentFormDto input)
    {
        var order = await _orderRepository.GetAsync(orderId);
        var userId = (CurrentUser.Id ?? Guid.Empty);
        if (order.StudentId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.OrderNotFound);

        if (order.Status == OrderStatus.Cancelled)
            throw new BusinessException(FitliyoDomainErrorCodes.OrderCannotBeCancelled);

        order.StudentFormData = input.FormData;
        order.StudentFormSubmittedAt = DateTime.Now;
        await _orderRepository.UpdateAsync(order);

        Logger.LogInformation("Sipariş öğrenci formu güncellendi: {OrderId}", orderId);
        return await GetAsync(orderId);
    }

    [Authorize]
    public async Task<OrderDto> UpdateOrderDeliveryAsync(Guid orderId, UpdateOrderDeliveryDto input)
    {
        var order = await _orderRepository.GetAsync(orderId);
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (trainerProfile == null || trainerProfile.Id != order.TrainerProfileId)
            await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);

        if (!string.IsNullOrWhiteSpace(input.TrainerProgramNotes))
            order.TrainerProgramNotes = input.TrainerProgramNotes;
        if (input.ProgramAttachmentUrl != null)
            order.ProgramAttachmentUrl = input.ProgramAttachmentUrl;
        if (input.MarkAsDelivered)
            order.ProgramDeliveredAt = DateTime.Now;

        await _orderRepository.UpdateAsync(order);

        Logger.LogInformation("Sipariş program teslimi güncellendi: {OrderId}", orderId);
        return await GetAsync(orderId);
    }

    private static string GenerateOrderNumber()
    {
        return $"FIT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }

    private async Task PopulateOrderDisplayNamesAsync(Order order, TrainerProfile trainerProfile)
    {
        var student = await _identityUserRepository.FindAsync(order.StudentId);
        var trainerUser = await _identityUserRepository.FindAsync(trainerProfile.UserId);
        var studentFullName = BuildFullName(student?.Name, student?.Surname);
        var trainerFullName = BuildFullName(trainerUser?.Name, trainerUser?.Surname);

        if (!string.IsNullOrWhiteSpace(studentFullName))
        {
            order.SetProperty("StudentFullName", studentFullName);
        }

        if (!string.IsNullOrWhiteSpace(trainerFullName))
        {
            order.SetProperty("TrainerFullName", trainerFullName);
        }
    }

    private async Task EnrichOrderDisplayNamesAsync(IReadOnlyList<Order> orders)
    {
        if (orders.Count == 0)
        {
            return;
        }

        var studentIds = orders.Select(x => x.StudentId).Distinct().ToList();
        var trainerProfileIds = orders.Select(x => x.TrainerProfileId).Distinct().ToList();

        var trainerProfilesQuery = await _trainerProfileRepository.GetQueryableAsync();
        var trainerProfiles = await AsyncExecuter.ToListAsync(trainerProfilesQuery.Where(x => trainerProfileIds.Contains(x.Id)));
        var trainerProfileMap = trainerProfiles.ToDictionary(x => x.Id, x => x.UserId);

        var userIds = studentIds
            .Concat(trainerProfiles.Select(x => x.UserId))
            .Distinct()
            .ToList();

        var usersQuery = await _identityUserRepository.GetQueryableAsync();
        var users = await AsyncExecuter.ToListAsync(usersQuery.Where(x => userIds.Contains(x.Id)));
        var userNameMap = users.ToDictionary(x => x.Id, x => BuildFullName(x.Name, x.Surname));

        var dirtyOrders = new List<Order>();
        foreach (var order in orders)
        {
            var studentFullName = order.GetProperty<string>("StudentFullName");
            if (string.IsNullOrWhiteSpace(studentFullName) && userNameMap.TryGetValue(order.StudentId, out var resolvedStudentName) && !string.IsNullOrWhiteSpace(resolvedStudentName))
            {
                order.SetProperty("StudentFullName", resolvedStudentName);
                dirtyOrders.Add(order);
            }

            var trainerFullName = order.GetProperty<string>("TrainerFullName");
            if (string.IsNullOrWhiteSpace(trainerFullName)
                && trainerProfileMap.TryGetValue(order.TrainerProfileId, out var trainerUserId)
                && userNameMap.TryGetValue(trainerUserId, out var resolvedTrainerName)
                && !string.IsNullOrWhiteSpace(resolvedTrainerName))
            {
                order.SetProperty("TrainerFullName", resolvedTrainerName);
                if (!dirtyOrders.Contains(order))
                {
                    dirtyOrders.Add(order);
                }
            }
        }

        if (dirtyOrders.Count > 0)
        {
            await _orderRepository.UpdateManyAsync(dirtyOrders);
        }
    }

    private OrderDto MapOrderToDto(Order order)
    {
        var dto = ObjectMapper.Map<Order, OrderDto>(order);
        dto.StudentFullName = order.GetProperty<string>("StudentFullName");
        dto.TrainerFullName = order.GetProperty<string>("TrainerFullName");
        return dto;
    }

    private static string? BuildFullName(string? name, string? surname)
    {
        var fullName = $"{name} {surname}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
    }
}
