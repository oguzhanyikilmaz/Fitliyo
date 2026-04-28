using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Kullanıcının tanımladığı gıda (100g başına değerler).
/// </summary>
public class UserFoodItem : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    [Required]
    [StringLength(WellnessConsts.MaxFoodNameLength)]
    public string Name { get; set; } = string.Empty;

    public decimal KcalPer100G { get; set; }
    public decimal ProteinPer100G { get; set; }
    public decimal CarbsPer100G { get; set; }
    public decimal FatPer100G { get; set; }

    protected UserFoodItem()
    {
    }

    public UserFoodItem(Guid id, Guid userId, string name) : base(id)
    {
        UserId = userId;
        Name = name;
    }
}
