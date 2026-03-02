using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Content;
using Fitliyo.Enums;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Content;

/// <summary>
/// Blog yazısı — İçerik modülü
/// </summary>
public class BlogPost : FullAuditedAggregateRoot<Guid>
{
    [Required]
    [StringLength(BlogPostConsts.MaxTitleLength)]
    public string Title { get; set; } = default!;

    [Required]
    [StringLength(BlogPostConsts.MaxSlugLength)]
    public string Slug { get; set; } = default!;

    [StringLength(BlogPostConsts.MaxSummaryLength)]
    public string? Summary { get; set; }

    [Required]
    [StringLength(BlogPostConsts.MaxBodyLength)]
    public string Body { get; set; } = default!;

    public BlogPostStatus Status { get; set; }

    public DateTime? PublishedAt { get; set; }

    [StringLength(BlogPostConsts.MaxAuthorNameLength)]
    public string? AuthorName { get; set; }

    public Guid? AuthorUserId { get; set; }

    [StringLength(BlogPostConsts.MaxFeaturedImageUrlLength)]
    public string? FeaturedImageUrl { get; set; }

    protected BlogPost()
    {
    }

    public BlogPost(Guid id, string title, string slug, string body)
        : base(id)
    {
        Title = title;
        Slug = slug;
        Body = body;
        Status = BlogPostStatus.Draft;
    }

    public void Publish()
    {
        Status = BlogPostStatus.Published;
        PublishedAt = DateTime.Now;
    }
}
