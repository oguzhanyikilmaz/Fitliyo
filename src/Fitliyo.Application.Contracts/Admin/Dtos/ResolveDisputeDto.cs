using System.ComponentModel.DataAnnotations;

namespace Fitliyo.Admin.Dtos;

public class ResolveDisputeDto
{
    [Required]
    [StringLength(1000)]
    public string ResolutionNote { get; set; } = default!;
}
