using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Tamamlanan antrenman günlüğü (tarih bazlı).
/// </summary>
public class UserWorkoutLog : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    public Guid? PersonalWorkoutProgramId { get; set; }

    /// <summary>Sadece tarih (saat 00:00) kullanılır</summary>
    public DateTime LogDate { get; set; }

    [StringLength(WellnessConsts.MaxWorkoutNoteLength)]
    public string? Notes { get; set; }

    /// <summary>Alt satırlardan hesaplanan toplam kcal (sunucu doldurur)</summary>
    public decimal TotalCaloriesBurned { get; set; }

    protected UserWorkoutLog()
    {
    }

    public UserWorkoutLog(Guid id, Guid userId, DateTime logDate) : base(id)
    {
        UserId = userId;
        LogDate = logDate.Date;
    }
}
