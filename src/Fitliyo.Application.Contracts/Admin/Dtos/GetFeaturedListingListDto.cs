using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Admin.Dtos;

public class GetFeaturedListingListDto : PagedAndSortedResultRequestDto
{
    public FeaturedListingPageType? PageType { get; set; }
    public bool? IsActive { get; set; }
}
