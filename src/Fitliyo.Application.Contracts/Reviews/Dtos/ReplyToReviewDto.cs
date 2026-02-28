using System.ComponentModel.DataAnnotations;
using Fitliyo.Reviews;

namespace Fitliyo.Reviews.Dtos;

public class ReplyToReviewDto
{
    [Required]
    [StringLength(ReviewConsts.MaxReplyLength)]
    public string Reply { get; set; } = default!;
}
