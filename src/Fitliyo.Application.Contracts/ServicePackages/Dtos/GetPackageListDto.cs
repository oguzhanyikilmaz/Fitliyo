using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.ServicePackages.Dtos;

public class GetPackageListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public Guid? TrainerProfileId { get; set; }
    public PackageType? PackageType { get; set; }
    public bool? IsOnline { get; set; }
    public bool? IsOnSite { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public Guid? CategoryId { get; set; }
}
