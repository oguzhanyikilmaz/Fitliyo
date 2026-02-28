using System;
using System.ComponentModel.DataAnnotations;

namespace Fitliyo.Orders.Dtos;

public class CreateOrderDto
{
    [Required]
    public Guid ServicePackageId { get; set; }

    [Range(1, 10)]
    public int Quantity { get; set; } = 1;

    public string? Notes { get; set; }
}
