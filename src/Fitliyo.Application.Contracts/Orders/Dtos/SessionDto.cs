using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Orders.Dtos;

public class SessionDto : EntityDto<Guid>
{
    public Guid OrderId { get; set; }
    public Guid TrainerProfileId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public SessionStatus Status { get; set; }
    public string? MeetingUrl { get; set; }
    public string? TrainerNotes { get; set; }
    public string? StudentNotes { get; set; }
    public int SequenceNumber { get; set; }
}
