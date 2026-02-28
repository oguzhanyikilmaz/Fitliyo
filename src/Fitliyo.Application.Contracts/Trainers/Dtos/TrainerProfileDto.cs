using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Trainers.Dtos;

public class TrainerProfileDto : EntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Slug { get; set; } = default!;
    public string? Bio { get; set; }
    public int ExperienceYears { get; set; }
    public TrainerType TrainerType { get; set; }
    public string? SpecialtyTags { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public bool IsOnlineAvailable { get; set; }
    public bool IsOnSiteAvailable { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviewCount { get; set; }
    public int TotalStudentCount { get; set; }
    public bool IsVerified { get; set; }
    public string? VerificationBadge { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; }
    public int ProfileCompletionPct { get; set; }
    public string? InstagramUrl { get; set; }
    public string? YoutubeUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// Denormalize — kullanıcı adı soyadı
    /// </summary>
    public string? TrainerFullName { get; set; }
    public string? ProfilePhotoUrl { get; set; }
}
