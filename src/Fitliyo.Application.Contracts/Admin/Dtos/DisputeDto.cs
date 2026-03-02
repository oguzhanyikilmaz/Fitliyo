using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Admin.Dtos;

public class DisputeDto : EntityDto<Guid>
{
    public Guid OrderId { get; set; }
    public Guid OpenedByUserId { get; set; }
    public DisputeType DisputeType { get; set; }
    public string Description { get; set; } = default!;
    public DisputeStatus Status { get; set; }
    public string? ResolutionNote { get; set; }
    public Guid? ResolvedByUserId { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreationTime { get; set; }
}
