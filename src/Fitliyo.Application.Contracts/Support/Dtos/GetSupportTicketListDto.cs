using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Support.Dtos;

public class GetSupportTicketListDto : PagedAndSortedResultRequestDto
{
    public SupportTicketStatus? Status { get; set; }
    public SupportTicketCategory? Category { get; set; }
}
