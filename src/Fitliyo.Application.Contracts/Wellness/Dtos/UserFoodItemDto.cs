using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class UserFoodItemDto : AuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal KcalPer100G { get; set; }
    public decimal ProteinPer100G { get; set; }
    public decimal CarbsPer100G { get; set; }
    public decimal FatPer100G { get; set; }
}
