using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Reviews;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Reviews;

/// <summary>
/// Değerlendirme — Öğrencinin tamamlanan sipariş için eğitmeni puanlaması
/// </summary>
public class Review : FullAuditedAggregateRoot<Guid>
{
    [Required]
    public Guid OrderId { get; private set; }

    [Required]
    public Guid StudentId { get; private set; }

    [Required]
    public Guid TrainerProfileId { get; private set; }

    /// <summary>
    /// Hangi paket için yorum (opsiyonel)
    /// </summary>
    public Guid? ServicePackageId { get; set; }

    /// <summary>
    /// Genel puan (1-5) — geriye uyumluluk için
    /// </summary>
    [Range(ReviewConsts.MinRating, ReviewConsts.MaxRating)]
    public int Rating { get; set; }

    /// <summary>
    /// Ortalama puan (alt kriterlerin ortalaması veya tek puan)
    /// </summary>
    public decimal OverallRating { get; set; }

    [Range(ReviewConsts.MinRating, ReviewConsts.MaxRating)]
    public int? CommunicationRating { get; set; }

    [Range(ReviewConsts.MinRating, ReviewConsts.MaxRating)]
    public int? ExpertiseRating { get; set; }

    [Range(ReviewConsts.MinRating, ReviewConsts.MaxRating)]
    public int? ValueForMoneyRating { get; set; }

    [Range(ReviewConsts.MinRating, ReviewConsts.MaxRating)]
    public int? PunctualityRating { get; set; }

    public bool IsVerifiedPurchase { get; set; }

    public bool IsPublished { get; set; } = true;

    public int HelpfulCount { get; set; }

    [StringLength(ReviewConsts.MaxCommentLength)]
    public string? Comment { get; set; }

    /// <summary>
    /// Eğitmenin yanıtı
    /// </summary>
    [StringLength(ReviewConsts.MaxReplyLength)]
    public string? TrainerReply { get; set; }

    public DateTime? TrainerReplyDate { get; set; }

    /// <summary>
    /// Admin tarafından gizlendi mi (uygunsuz içerik)
    /// </summary>
    public bool IsHidden { get; set; }

    protected Review()
    {
    }

    public Review(Guid id, Guid orderId, Guid studentId, Guid trainerProfileId, int rating)
        : base(id)
    {
        OrderId = orderId;
        StudentId = studentId;
        TrainerProfileId = trainerProfileId;
        Rating = Check.Range(rating, nameof(rating), ReviewConsts.MinRating, ReviewConsts.MaxRating);
        OverallRating = rating;
    }

    public void SetTrainerReply(string reply)
    {
        TrainerReply = Check.NotNullOrWhiteSpace(reply, nameof(reply), ReviewConsts.MaxReplyLength);
        TrainerReplyDate = DateTime.Now;
    }
}
