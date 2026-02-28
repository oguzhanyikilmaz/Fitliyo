using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;
using Fitliyo.Trainers;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Trainers;

/// <summary>
/// Eğitmen galeri öğesi (resim/video)
/// </summary>
public class TrainerGallery : FullAuditedEntity<Guid>
{
    [Required]
    public Guid TrainerProfileId { get; private set; }

    public MediaType MediaType { get; set; }

    [Required]
    [StringLength(TrainerConsts.MaxMediaUrlLength)]
    public string MediaUrl { get; set; } = default!;

    [StringLength(TrainerConsts.MaxThumbnailUrlLength)]
    public string? ThumbnailUrl { get; set; }

    [StringLength(TrainerConsts.MaxCaptionLength)]
    public string? Caption { get; set; }

    public int SortOrder { get; set; }

    /// <summary>
    /// Kapak resmi mi
    /// </summary>
    public bool IsCoverImage { get; set; }

    protected TrainerGallery()
    {
    }

    public TrainerGallery(Guid id, Guid trainerProfileId, MediaType mediaType, string mediaUrl)
        : base(id)
    {
        TrainerProfileId = trainerProfileId;
        MediaType = mediaType;
        MediaUrl = Check.NotNullOrWhiteSpace(mediaUrl, nameof(mediaUrl), TrainerConsts.MaxMediaUrlLength);
    }
}
