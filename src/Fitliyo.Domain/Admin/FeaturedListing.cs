using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Admin;
using Fitliyo.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Admin;

/// <summary>
/// Öne çıkan listeleme — Belirli sayfalarda öne çıkarılacak paket veya eğitmen
/// </summary>
public class FeaturedListing : FullAuditedEntity<Guid>
{
    public FeaturedListingPageType PageType { get; set; }

    public Guid? TrainerProfileId { get; set; }

    public Guid? ServicePackageId { get; set; }

    public int SortOrder { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(FeaturedListingConsts.MaxNoteLength)]
    public string? AdminNote { get; set; }

    protected FeaturedListing()
    {
    }

    public FeaturedListing(
        Guid id,
        FeaturedListingPageType pageType,
        int sortOrder,
        Guid? trainerProfileId = null,
        Guid? servicePackageId = null)
        : base(id)
    {
        PageType = pageType;
        SortOrder = sortOrder;
        TrainerProfileId = trainerProfileId;
        ServicePackageId = servicePackageId;
    }
}
