using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.ServicePackages.Dtos;

public class ServicePackageDto : EntityDto<Guid>
{
    public Guid TrainerProfileId { get; set; }
    public PackageType PackageType { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Currency { get; set; } = "TRY";
    public int? DurationDays { get; set; }
    public int? SessionCount { get; set; }
    public int? SessionDurationMinutes { get; set; }
    public int MaxStudents { get; set; }
    public bool IsActive { get; set; }
    public bool IsOnline { get; set; }
    public bool IsOnSite { get; set; }
    public int CancellationHours { get; set; }
    public string? CancellationPolicy { get; set; }
    public string? WhatIsIncluded { get; set; }
    public string? WhatIsNotIncluded { get; set; }
    public string? Tags { get; set; }
    public int TotalSalesCount { get; set; }
    public decimal AverageRating { get; set; }
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Denormalize — eğitmen adı
    /// </summary>
    public string? TrainerFullName { get; set; }
    public string? TrainerSlug { get; set; }
}
