using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Subscriptions.Dtos;

public class TrainerSubscriptionDto : EntityDto<Guid>
{
    public Guid TrainerProfileId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAutoRenew { get; set; }
    public decimal PaidAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public DateTime? CancelledAt { get; set; }

    public string? PlanName { get; set; }
    public SubscriptionTier? PlanTier { get; set; }
}
