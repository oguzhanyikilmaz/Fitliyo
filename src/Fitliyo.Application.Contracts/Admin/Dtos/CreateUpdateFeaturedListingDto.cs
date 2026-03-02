using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;

namespace Fitliyo.Admin.Dtos;

public class CreateUpdateFeaturedListingDto
{
    public FeaturedListingPageType PageType { get; set; }

    public Guid? TrainerProfileId { get; set; }

    public Guid? ServicePackageId { get; set; }

    public int SortOrder { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    [StringLength(256)]
    public string? AdminNote { get; set; }
}
