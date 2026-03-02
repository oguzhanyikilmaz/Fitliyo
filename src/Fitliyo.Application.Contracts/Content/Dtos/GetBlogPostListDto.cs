using Fitliyo.Enums;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Content.Dtos;

public class GetBlogPostListDto : PagedAndSortedResultRequestDto
{
    public BlogPostStatus? Status { get; set; }
    public string? Slug { get; set; }
}
