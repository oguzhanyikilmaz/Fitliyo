using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Subscriptions;

namespace Fitliyo.Subscriptions.Dtos;

public class CreateUpdateSubscriptionPlanDto
{
    [Required]
    [StringLength(SubscriptionConsts.MaxPlanNameLength)]
    public string Name { get; set; } = default!;

    [StringLength(SubscriptionConsts.MaxPlanDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    public SubscriptionTier Tier { get; set; }

    [Required]
    public SubscriptionPlanType PlanType { get; set; }

    [Required]
    [Range(0, 99999.99)]
    public decimal Price { get; set; }

    public int MaxPackageCount { get; set; }

    [Range(0, 1)]
    public decimal CommissionRate { get; set; }

    public bool HasFeaturedListing { get; set; }
    public bool HasPrioritySupport { get; set; }
    public bool HasAdvancedAnalytics { get; set; }
    public string? FeaturesJson { get; set; }
    public int SortOrder { get; set; }
}
