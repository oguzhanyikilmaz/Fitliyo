using System;
using System.Threading.Tasks;
using Fitliyo.Orders.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Orders;

public interface IOrderAppService : IApplicationService
{
    Task<OrderDto> GetAsync(Guid id);

    Task<PagedResultDto<OrderDto>> GetMyOrdersAsync(GetOrderListDto input);

    Task<PagedResultDto<OrderDto>> GetTrainerOrdersAsync(GetOrderListDto input);

    Task<OrderDto> CreateAsync(CreateOrderDto input);

    Task<OrderDto> CancelAsync(Guid id, string? reason);

    Task<OrderDto> CompleteAsync(Guid id);

    Task<PagedResultDto<SessionDto>> GetSessionsAsync(Guid orderId);
}
