using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Trainers;

namespace Fitliyo.Trainers.Dtos;

public class CreateUpdateTrainerGalleryDto
{
    [Required]
    public MediaType MediaType { get; set; }

    [Required]
    [StringLength(TrainerConsts.MaxMediaUrlLength)]
    public string MediaUrl { get; set; } = default!;

    [StringLength(TrainerConsts.MaxThumbnailUrlLength)]
    public string? ThumbnailUrl { get; set; }

    [StringLength(TrainerConsts.MaxCaptionLength)]
    public string? Caption { get; set; }

    public int SortOrder { get; set; }

    public bool IsCoverImage { get; set; }
}
