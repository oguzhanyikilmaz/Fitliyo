using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Payments.Dtos;

public class GetWithdrawalRequestListDto : PagedAndSortedResultRequestDto
{
    public WithdrawalRequestStatus? Status { get; set; }
}
