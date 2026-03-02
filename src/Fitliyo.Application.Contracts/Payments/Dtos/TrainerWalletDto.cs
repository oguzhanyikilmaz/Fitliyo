using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Payments.Dtos;

public class TrainerWalletDto : EntityDto<Guid>
{
    public Guid TrainerProfileId { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal TotalEarned { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public DateTime? LastPayoutAt { get; set; }
}
