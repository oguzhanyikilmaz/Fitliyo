using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class GetPersonalWorkoutProgramListDto : PagedAndSortedResultRequestDto
{
    public bool? IncludeArchived { get; set; }
}
