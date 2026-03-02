using System;
using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Content.Dtos;

public class BlogPostDto : EntityDto<Guid>
{
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? Summary { get; set; }
    public string Body { get; set; } = default!;
    public BlogPostStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string? AuthorName { get; set; }
    public Guid? AuthorUserId { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public DateTime CreationTime { get; set; }
}
