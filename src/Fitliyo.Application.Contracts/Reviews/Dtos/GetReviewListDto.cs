using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Reviews.Dtos;

public class GetReviewListDto : PagedAndSortedResultRequestDto
{
    public Guid? TrainerProfileId { get; set; }
    public int? MinRating { get; set; }
}
