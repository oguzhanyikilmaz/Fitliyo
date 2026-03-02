using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Payments.Dtos;

public class WalletTransactionDto : EntityDto<Guid>
{
    public Guid TrainerWalletId { get; set; }
    public WalletTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = default!;
    public Guid? ReferenceId { get; set; }
    public decimal BalanceAfter { get; set; }
    public DateTime CreationTime { get; set; }
}
