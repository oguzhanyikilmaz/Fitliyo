using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Reviews.Dtos;

public class ReviewDto : EntityDto<Guid>
{
    public Guid OrderId { get; set; }
    public Guid StudentId { get; set; }
    public Guid TrainerProfileId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? TrainerReply { get; set; }
    public DateTime? TrainerReplyDate { get; set; }
    public bool IsHidden { get; set; }
    public DateTime CreationTime { get; set; }

    public string? StudentFullName { get; set; }
}
