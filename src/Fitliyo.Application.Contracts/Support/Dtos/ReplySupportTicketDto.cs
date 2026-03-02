using System.ComponentModel.DataAnnotations;

namespace Fitliyo.Support.Dtos;

public class ReplySupportTicketDto
{
    [Required]
    [StringLength(2000)]
    public string AdminReply { get; set; } = default!;
}
