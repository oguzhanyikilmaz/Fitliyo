using System;
using System.Threading.Tasks;
using Fitliyo.Orders;
using Fitliyo.Orders.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/orders")]
public class OrderController : FitliyoController, IOrderAppService
{
    private readonly IOrderAppService _orderAppService;

    public OrderController(IOrderAppService orderAppService)
    {
        _orderAppService = orderAppService;
    }

    [HttpGet("{id}")]
    public Task<OrderDto> GetAsync(Guid id)
    {
        return _orderAppService.GetAsync(id);
    }

    [HttpGet("my-orders")]
    public Task<PagedResultDto<OrderDto>> GetMyOrdersAsync([FromQuery] GetOrderListDto input)
    {
        return _orderAppService.GetMyOrdersAsync(input);
    }

    [HttpGet("trainer-orders")]
    public Task<PagedResultDto<OrderDto>> GetTrainerOrdersAsync([FromQuery] GetOrderListDto input)
    {
        return _orderAppService.GetTrainerOrdersAsync(input);
    }

    [HttpPost]
    public Task<OrderDto> CreateAsync(CreateOrderDto input)
    {
        return _orderAppService.CreateAsync(input);
    }

    [HttpPost("{id}/cancel")]
    public Task<OrderDto> CancelAsync(Guid id, [FromQuery] string? reason)
    {
        return _orderAppService.CancelAsync(id, reason);
    }

    [HttpPost("{id}/complete")]
    public Task<OrderDto> CompleteAsync(Guid id)
    {
        return _orderAppService.CompleteAsync(id);
    }

    [HttpGet("{orderId}/sessions")]
    public Task<PagedResultDto<SessionDto>> GetSessionsAsync(Guid orderId)
    {
        return _orderAppService.GetSessionsAsync(orderId);
    }
}
