using System.ComponentModel.DataAnnotations;
using Fitliyo.Wellness;

namespace Fitliyo.Wellness.Dtos;

public class CreateUpdateUserFoodItemDto
{
    [Required]
    [StringLength(WellnessConsts.MaxFoodNameLength)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 10000)]
    public decimal KcalPer100G { get; set; }

    [Range(0, 1000)]
    public decimal ProteinPer100G { get; set; }

    [Range(0, 1000)]
    public decimal CarbsPer100G { get; set; }

    [Range(0, 1000)]
    public decimal FatPer100G { get; set; }
}
