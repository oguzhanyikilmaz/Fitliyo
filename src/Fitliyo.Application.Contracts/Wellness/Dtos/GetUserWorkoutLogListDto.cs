using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class GetUserWorkoutLogListDto : PagedAndSortedResultRequestDto
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
