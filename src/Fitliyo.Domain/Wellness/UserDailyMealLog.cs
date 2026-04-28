using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Belirli bir günün besin kayıt üst bilgisi (kullanıcı + tarih tekil).
/// </summary>
public class UserDailyMealLog : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    public DateTime LogDate { get; set; }

    [StringLength(WellnessConsts.MaxWorkoutNoteLength)]
    public string? Notes { get; set; }

    protected UserDailyMealLog()
    {
    }

    public UserDailyMealLog(Guid id, Guid userId, DateTime logDate) : base(id)
    {
        UserId = userId;
        LogDate = logDate.Date;
    }
}
