using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class GetPersonalNutritionPlanListDto : PagedAndSortedResultRequestDto
{
    public bool? ActiveOnly { get; set; }
}
