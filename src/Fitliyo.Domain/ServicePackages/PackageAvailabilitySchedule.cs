using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.ServicePackages;

/// <summary>
/// Paket müsaitlik takvimi (haftalık tekrarlayan slotlar)
/// </summary>
public class PackageAvailabilitySchedule : FullAuditedEntity<Guid>
{
    [Required]
    public Guid ServicePackageId { get; private set; }

    public DayOfWeekEnum DayOfWeek { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public bool IsAvailable { get; set; }

    /// <summary>
    /// Slot süresi (dakika)
    /// </summary>
    public int SlotDurationMinutes { get; set; }

    protected PackageAvailabilitySchedule()
    {
    }

    public PackageAvailabilitySchedule(Guid id, Guid servicePackageId, DayOfWeekEnum dayOfWeek,
        TimeSpan startTime, TimeSpan endTime, int slotDurationMinutes)
        : base(id)
    {
        ServicePackageId = servicePackageId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        SlotDurationMinutes = slotDurationMinutes;
        IsAvailable = true;
    }
}
