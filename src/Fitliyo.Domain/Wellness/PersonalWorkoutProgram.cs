using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Wellness;

/// <summary>
/// Kullanıcının kendi oluşturduğu antrenman programı (şablon).
/// </summary>
public class PersonalWorkoutProgram : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid UserId { get; private set; }

    [Required]
    [StringLength(WellnessConsts.MaxTitleLength)]
    public string Title { get; set; } = string.Empty;

    [StringLength(WellnessConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    /// <summary>1–7: haftanın günü; null: genel / sırayla</summary>
    public int? WeekdayIndex { get; set; }

    public bool IsArchived { get; set; }

    protected PersonalWorkoutProgram()
    {
    }

    public PersonalWorkoutProgram(Guid id, Guid userId, string title) : base(id)
    {
        UserId = userId;
        Title = title;
    }
}
