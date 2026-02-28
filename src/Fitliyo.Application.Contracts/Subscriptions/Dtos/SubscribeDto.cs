using System;
using System.ComponentModel.DataAnnotations;

namespace Fitliyo.Subscriptions.Dtos;

public class SubscribeDto
{
    [Required]
    public Guid SubscriptionPlanId { get; set; }

    public bool IsAutoRenew { get; set; } = true;
}
