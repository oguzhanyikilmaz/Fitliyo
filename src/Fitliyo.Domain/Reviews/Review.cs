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
    /// Puan (1-5)
    /// </summary>
    [Range(ReviewConsts.MinRating, ReviewConsts.MaxRating)]
    public int Rating { get; set; }

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
    }

    public void SetTrainerReply(string reply)
    {
        TrainerReply = Check.NotNullOrWhiteSpace(reply, nameof(reply), ReviewConsts.MaxReplyLength);
        TrainerReplyDate = DateTime.Now;
    }
}
