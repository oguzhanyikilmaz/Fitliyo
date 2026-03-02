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

    /// <summary>
    /// Öğrenci: Bu sipariş için eğitmene ileteceği bilgileri günceller (kan değerleri, hedefler vb.).
    /// </summary>
    Task<OrderDto> UpdateStudentFormAsync(Guid orderId, UpdateOrderStudentFormDto input);

    /// <summary>
    /// Eğitmen: Bu siparişe program notu ve/veya dosya linki ekler; isteğe bağlı olarak teslim edildi işaretler.
    /// </summary>
    Task<OrderDto> UpdateOrderDeliveryAsync(Guid orderId, UpdateOrderDeliveryDto input);
}
