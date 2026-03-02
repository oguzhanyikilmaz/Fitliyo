using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Payments.Dtos;

public class GetPaymentListDto : PagedAndSortedResultRequestDto
{
    public Guid? OrderId { get; set; }
    public PaymentRecordStatus? Status { get; set; }
}
