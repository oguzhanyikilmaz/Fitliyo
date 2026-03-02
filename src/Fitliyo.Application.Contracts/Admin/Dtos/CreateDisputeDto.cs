using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;

namespace Fitliyo.Admin.Dtos;

public class CreateDisputeDto
{
    [Required]
    public Guid OrderId { get; set; }

    public DisputeType DisputeType { get; set; }

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = default!;
}
