using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Reviews;

namespace Fitliyo.Reviews.Dtos;

public class CreateReviewDto
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [Range(ReviewConsts.MinRating, ReviewConsts.MaxRating)]
    public int Rating { get; set; }

    [StringLength(ReviewConsts.MaxCommentLength)]
    public string? Comment { get; set; }
}
