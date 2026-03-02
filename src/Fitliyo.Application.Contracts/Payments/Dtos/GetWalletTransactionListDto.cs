using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Payments.Dtos;

public class GetWalletTransactionListDto : PagedAndSortedResultRequestDto
{
    public Guid? TrainerWalletId { get; set; }
    public WalletTransactionType? TransactionType { get; set; }
}
