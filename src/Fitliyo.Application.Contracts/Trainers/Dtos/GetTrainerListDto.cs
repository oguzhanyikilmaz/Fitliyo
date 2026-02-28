using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Trainers.Dtos;

public class GetTrainerListDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
    public TrainerType? TrainerType { get; set; }
    public string? City { get; set; }
    public bool? IsOnlineAvailable { get; set; }
    public bool? IsOnSiteAvailable { get; set; }
    public bool? IsVerified { get; set; }
    public Guid? CategoryId { get; set; }
}
