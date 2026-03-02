using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Reviews;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Reviews;

/// <summary>
/// Değerlendirme "faydalı" oyu — Kullanıcıların yoruma faydalı demesi
/// </summary>
public class ReviewHelpfulVote : FullAuditedEntity<Guid>
{
    [Required]
    public Guid ReviewId { get; private set; }

    [Required]
    public Guid VoterUserId { get; private set; }

    public bool IsHelpful { get; set; }

    protected ReviewHelpfulVote()
    {
    }

    public ReviewHelpfulVote(Guid id, Guid reviewId, Guid voterUserId, bool isHelpful)
        : base(id)
    {
        ReviewId = reviewId;
        VoterUserId = voterUserId;
        IsHelpful = isHelpful;
    }
}
