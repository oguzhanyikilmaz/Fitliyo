using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;

namespace Fitliyo.Content.Dtos;

public class CreateUpdateBlogPostDto
{
    [Required]
    [StringLength(256)]
    public string Title { get; set; } = default!;

    [Required]
    [StringLength(256)]
    public string Slug { get; set; } = default!;

    [StringLength(500)]
    public string? Summary { get; set; }

    [Required]
    [StringLength(50_000)]
    public string Body { get; set; } = default!;

    public BlogPostStatus Status { get; set; }

    [StringLength(128)]
    public string? AuthorName { get; set; }

    [StringLength(512)]
    public string? FeaturedImageUrl { get; set; }
}
