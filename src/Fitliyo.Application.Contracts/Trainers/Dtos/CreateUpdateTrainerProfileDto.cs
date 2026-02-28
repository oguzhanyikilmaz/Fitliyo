using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Trainers;

namespace Fitliyo.Trainers.Dtos;

public class CreateUpdateTrainerProfileDto
{
    [Required]
    [StringLength(TrainerConsts.MaxSlugLength)]
    public string Slug { get; set; } = default!;

    [StringLength(TrainerConsts.MaxBioLength)]
    public string? Bio { get; set; }

    [Range(0, 50)]
    public int ExperienceYears { get; set; }

    [Required]
    public TrainerType TrainerType { get; set; }

    public string? SpecialtyTags { get; set; }

    [StringLength(TrainerConsts.MaxCityLength)]
    public string? City { get; set; }

    [StringLength(TrainerConsts.MaxDistrictLength)]
    public string? District { get; set; }

    public bool IsOnlineAvailable { get; set; }

    public bool IsOnSiteAvailable { get; set; }

    [StringLength(TrainerConsts.MaxInstagramUrlLength)]
    public string? InstagramUrl { get; set; }

    [StringLength(TrainerConsts.MaxYoutubeUrlLength)]
    public string? YoutubeUrl { get; set; }

    [StringLength(TrainerConsts.MaxWebsiteUrlLength)]
    public string? WebsiteUrl { get; set; }
}
