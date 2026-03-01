using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.ServicePackages;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.ServicePackages;

/// <summary>
/// Eğitmenin müsait olmadığı tarihler (tatil, özel gün)
/// </summary>
public class PackageUnavailableDate : FullAuditedEntity<Guid>
{
    [Required]
    public Guid TrainerProfileId { get; private set; }

    public DateTime UnavailableDate { get; set; }

    [StringLength(PackageConsts.MaxReasonLength)]
    public string? Reason { get; set; }

    protected PackageUnavailableDate()
    {
    }

    public PackageUnavailableDate(Guid id, Guid trainerProfileId, DateTime unavailableDate)
        : base(id)
    {
        TrainerProfileId = trainerProfileId;
        UnavailableDate = unavailableDate;
    }
}
