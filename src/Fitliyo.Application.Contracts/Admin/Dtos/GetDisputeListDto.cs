using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Admin.Dtos;

public class GetDisputeListDto : PagedAndSortedResultRequestDto
{
    public DisputeStatus? Status { get; set; }
    public Guid? OrderId { get; set; }
}
