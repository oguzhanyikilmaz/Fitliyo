using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class GetUserFoodItemListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
