using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Orders.Dtos;

public class GetOrderListDto : PagedAndSortedResultRequestDto
{
    public OrderStatus? Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
}
