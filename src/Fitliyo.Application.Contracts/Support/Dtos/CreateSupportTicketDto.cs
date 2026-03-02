using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;

namespace Fitliyo.Support.Dtos;

public class CreateSupportTicketDto
{
    [Required]
    [StringLength(256)]
    public string Subject { get; set; } = default!;

    [Required]
    [StringLength(4000)]
    public string Message { get; set; } = default!;

    public SupportTicketCategory Category { get; set; }

    public Guid? OrderId { get; set; }
}
