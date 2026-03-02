using System;
using System.Threading.Tasks;
using Fitliyo.Content.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Content;

public interface IBlogPostAppService : IApplicationService
{
    Task<BlogPostDto> GetAsync(Guid id);
    Task<BlogPostDto?> GetBySlugAsync(string slug);
    Task<PagedResultDto<BlogPostDto>> GetListAsync(GetBlogPostListDto input);
    Task<PagedResultDto<BlogPostDto>> GetPublishedListAsync(GetBlogPostListDto input);
    Task<BlogPostDto> CreateAsync(CreateUpdateBlogPostDto input);
    Task<BlogPostDto> UpdateAsync(Guid id, CreateUpdateBlogPostDto input);
    Task DeleteAsync(Guid id);
    Task<BlogPostDto> PublishAsync(Guid id);
}
