using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Admin.Dtos;

public class FeaturedListingDto : EntityDto<Guid>
{
    public FeaturedListingPageType PageType { get; set; }
    public Guid? TrainerProfileId { get; set; }
    public Guid? ServicePackageId { get; set; }
    public int SortOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? AdminNote { get; set; }
}
