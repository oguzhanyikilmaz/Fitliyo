using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Wellness.Dtos;

public class UserDailyMealLogDto : AuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public DateTime LogDate { get; set; }
    public string? Notes { get; set; }
    public List<UserMealLogEntryDto> Entries { get; set; } = new();
    public decimal TotalCalories { get; set; }
    public decimal TotalProteinG { get; set; }
    public decimal TotalCarbsG { get; set; }
    public decimal TotalFatG { get; set; }
}
