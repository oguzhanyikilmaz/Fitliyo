using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Orders.Dtos;
using Fitliyo.Packages;
using Fitliyo.Permissions;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Orders;

[Authorize]
public class OrderAppService : FitliyoAppService, IOrderAppService
{
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<Session, Guid> _sessionRepository;
    private readonly IRepository<ServicePackage, Guid> _packageRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly FitliyoApplicationMappers _mapper;

    public OrderAppService(
        IRepository<Order, Guid> orderRepository,
        IRepository<Session, Guid> sessionRepository,
        IRepository<ServicePackage, Guid> packageRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        FitliyoApplicationMappers mapper)
    {
        _orderRepository = orderRepository;
        _sessionRepository = sessionRepository;
        _packageRepository = packageRepository;
        _trainerProfileRepository = trainerProfileRepository;
        _mapper = mapper;
    }

    [Authorize]
    public async Task<OrderDto> GetAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        var userId = CurrentUser.GetId();

        if (order.StudentId != userId)
        {
            var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
            if (trainerProfile == null || trainerProfile.Id != order.TrainerProfileId)
            {
                await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);
            }
        }

        return _mapper.OrderToDto(order);
    }

    [Authorize]
    public async Task<PagedResultDto<OrderDto>> GetMyOrdersAsync(GetOrderListDto input)
    {
        var userId = CurrentUser.GetId();
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

        return new PagedResultDto<OrderDto>(totalCount, entities.Select(_mapper.OrderToDto).ToList());
    }

    [Authorize]
    public async Task<PagedResultDto<OrderDto>> GetTrainerOrdersAsync(GetOrderListDto input)
    {
        var userId = CurrentUser.GetId();
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

        return new PagedResultDto<OrderDto>(totalCount, entities.Select(_mapper.OrderToDto).ToList());
    }

    [Authorize]
    public async Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        var userId = CurrentUser.GetId();
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

        await _orderRepository.InsertAsync(order);
        Logger.LogInformation("Sipariş oluşturuldu: {OrderId}, {OrderNumber}, Öğrenci: {StudentId}", order.Id, orderNumber, userId);

        return _mapper.OrderToDto(order);
    }

    [Authorize]
    public async Task<OrderDto> CancelAsync(Guid id, string? reason)
    {
        var order = await _orderRepository.GetAsync(id);
        var userId = CurrentUser.GetId();

        if (order.StudentId != userId)
            await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);

        if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.Refunded)
            throw new BusinessException(FitliyoDomainErrorCodes.OrderCannotBeCancelled);

        order.Cancel(reason);

        if (order.PaymentStatus == PaymentStatus.Escrow)
            order.PaymentStatus = PaymentStatus.Refunded;

        await _orderRepository.UpdateAsync(order);
        Logger.LogInformation("Sipariş iptal edildi: {OrderId}, Sebep: {Reason}", id, reason);

        return _mapper.OrderToDto(order);
    }

    [Authorize]
    public async Task<OrderDto> CompleteAsync(Guid id)
    {
        var order = await _orderRepository.GetAsync(id);
        var userId = CurrentUser.GetId();

        var trainerProfile = await _trainerProfileRepository.GetAsync(order.TrainerProfileId);
        if (trainerProfile.UserId != userId)
            await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);

        order.Complete();
        await _orderRepository.UpdateAsync(order);

        trainerProfile.TotalStudentCount++;
        await _trainerProfileRepository.UpdateAsync(trainerProfile);

        Logger.LogInformation("Sipariş tamamlandı: {OrderId}", id);

        return _mapper.OrderToDto(order);
    }

    [Authorize]
    public async Task<PagedResultDto<SessionDto>> GetSessionsAsync(Guid orderId)
    {
        var order = await _orderRepository.GetAsync(orderId);
        var userId = CurrentUser.GetId();

        if (order.StudentId != userId)
        {
            var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
            if (trainerProfile == null || trainerProfile.Id != order.TrainerProfileId)
                await AuthorizationService.CheckAsync(FitliyoPermissions.Admin.Dashboard);
        }

        var sessions = await _sessionRepository.GetListAsync(x => x.OrderId == orderId);
        var sorted = sessions.OrderBy(x => x.SequenceNumber).ToList();

        return new PagedResultDto<SessionDto>(sorted.Count, sorted.Select(_mapper.SessionToDto).ToList());
    }

    private static string GenerateOrderNumber()
    {
        return $"FIT-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }
}
