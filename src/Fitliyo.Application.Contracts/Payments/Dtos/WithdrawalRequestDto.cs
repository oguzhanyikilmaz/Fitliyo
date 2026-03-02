using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Payments.Dtos;

public class WithdrawalRequestDto : EntityDto<Guid>
{
    public Guid TrainerWalletId { get; set; }
    public decimal Amount { get; set; }
    public WithdrawalRequestStatus Status { get; set; }
    public string Iban { get; set; } = default!;
    public string AccountHolderName { get; set; } = default!;
    public string? AdminNote { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreationTime { get; set; }
}
