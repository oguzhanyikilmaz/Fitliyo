using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Trainers.Dtos;

public class TrainerGalleryDto : EntityDto<Guid>
{
    public Guid TrainerProfileId { get; set; }
    public MediaType MediaType { get; set; }
    public string MediaUrl { get; set; } = default!;
    public string? ThumbnailUrl { get; set; }
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
    public bool IsCoverImage { get; set; }
}
