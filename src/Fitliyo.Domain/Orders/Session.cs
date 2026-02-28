using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Orders;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Orders;

/// <summary>
/// Ders seansı — Sipariş içindeki bireysel ders
/// </summary>
public class Session : FullAuditedEntity<Guid>
{
    [Required]
    public Guid OrderId { get; private set; }

    [Required]
    public Guid TrainerProfileId { get; private set; }

    [Required]
    public Guid StudentId { get; private set; }

    /// <summary>
    /// Planlanan başlangıç zamanı
    /// </summary>
    public DateTime ScheduledStartTime { get; set; }

    /// <summary>
    /// Planlanan bitiş zamanı
    /// </summary>
    public DateTime ScheduledEndTime { get; set; }

    /// <summary>
    /// Gerçekleşen başlangıç
    /// </summary>
    public DateTime? ActualStartTime { get; set; }

    /// <summary>
    /// Gerçekleşen bitiş
    /// </summary>
    public DateTime? ActualEndTime { get; set; }

    public SessionStatus Status { get; set; }

    /// <summary>
    /// Online ders bağlantısı
    /// </summary>
    [StringLength(OrderConsts.MaxMeetingUrlLength)]
    public string? MeetingUrl { get; set; }

    /// <summary>
    /// Seans notları
    /// </summary>
    [StringLength(OrderConsts.MaxSessionNotesLength)]
    public string? TrainerNotes { get; set; }

    [StringLength(OrderConsts.MaxSessionNotesLength)]
    public string? StudentNotes { get; set; }

    /// <summary>
    /// Seans sıra numarası (1, 2, 3...)
    /// </summary>
    public int SequenceNumber { get; set; }

    protected Session()
    {
    }

    public Session(
        Guid id,
        Guid orderId,
        Guid trainerProfileId,
        Guid studentId,
        DateTime scheduledStartTime,
        DateTime scheduledEndTime,
        int sequenceNumber)
        : base(id)
    {
        OrderId = orderId;
        TrainerProfileId = trainerProfileId;
        StudentId = studentId;
        ScheduledStartTime = scheduledStartTime;
        ScheduledEndTime = scheduledEndTime;
        SequenceNumber = sequenceNumber;
        Status = SessionStatus.Scheduled;
    }
}
