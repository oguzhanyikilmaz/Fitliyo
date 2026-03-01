using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.ServicePackages;

namespace Fitliyo.ServicePackages.Dtos;

public class CreateUpdateServicePackageDto
{
    [Required]
    public PackageType PackageType { get; set; }

    [Required]
    [StringLength(PackageConsts.MaxTitleLength)]
    public string Title { get; set; } = default!;

    [StringLength(PackageConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    [Range(0.01, 999999.99)]
    public decimal? DiscountedPrice { get; set; }

    [StringLength(PackageConsts.MaxCurrencyLength)]
    public string Currency { get; set; } = PackageConsts.DefaultCurrency;

    [Range(1, 365)]
    public int? DurationDays { get; set; }

    [Range(1, 100)]
    public int? SessionCount { get; set; }

    [Range(15, 480)]
    public int? SessionDurationMinutes { get; set; }

    [Range(1, 100)]
    public int MaxStudents { get; set; } = 1;

    public bool IsOnline { get; set; }

    public bool IsOnSite { get; set; }

    [Range(0, 168)]
    public int CancellationHours { get; set; } = 24;

    [StringLength(PackageConsts.MaxCancellationPolicyLength)]
    public string? CancellationPolicy { get; set; }

    public string? WhatIsIncluded { get; set; }

    public string? WhatIsNotIncluded { get; set; }

    public string? Tags { get; set; }
}
