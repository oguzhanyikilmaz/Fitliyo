using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Content.Dtos;
using Fitliyo.Enums;
using Fitliyo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Content;

[Authorize]
public class BlogPostAppService : FitliyoAppService, IBlogPostAppService
{
    private readonly IRepository<BlogPost, Guid> _repository;

    public BlogPostAppService(IRepository<BlogPost, Guid> repository)
    {
        _repository = repository;
    }

    [Authorize(FitliyoPermissions.Content.Default)]
    public async Task<BlogPostDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<BlogPost, BlogPostDto>(entity);
    }

    [AllowAnonymous]
    public async Task<BlogPostDto?> GetBySlugAsync(string slug)
    {
        var entity = await _repository.FindAsync(x => x.Slug == slug);
        return entity == null ? null : ObjectMapper.Map<BlogPost, BlogPostDto>(entity);
    }

    [Authorize(FitliyoPermissions.Content.Default)]
    public async Task<PagedResultDto<BlogPostDto>> GetListAsync(GetBlogPostListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        if (input.Status.HasValue) queryable = queryable.Where(x => x.Status == input.Status.Value);
        if (!string.IsNullOrWhiteSpace(input.Slug)) queryable = queryable.Where(x => x.Slug == input.Slug);
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<BlogPostDto>(totalCount, items.Select(x => ObjectMapper.Map<BlogPost, BlogPostDto>(x)).ToList());
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<BlogPostDto>> GetPublishedListAsync(GetBlogPostListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        queryable = queryable.Where(x => x.Status == BlogPostStatus.Published);
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.PublishedAt);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<BlogPostDto>(totalCount, items.Select(x => ObjectMapper.Map<BlogPost, BlogPostDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Content.Create)]
    public async Task<BlogPostDto> CreateAsync(CreateUpdateBlogPostDto input)
    {
        var existing = await _repository.FindAsync(x => x.Slug == input.Slug);
        if (existing != null)
            throw new Volo.Abp.BusinessException(FitliyoDomainErrorCodes.BlogPostSlugAlreadyExists);

        var entity = new BlogPost(GuidGenerator.Create(), input.Title, input.Slug, input.Body);
        entity.Summary = input.Summary;
        entity.Status = input.Status;
        entity.AuthorName = input.AuthorName;
        entity.FeaturedImageUrl = input.FeaturedImageUrl;
        entity.AuthorUserId = CurrentUser.Id ?? Guid.Empty;
        await _repository.InsertAsync(entity);
        return ObjectMapper.Map<BlogPost, BlogPostDto>(entity);
    }

    [Authorize(FitliyoPermissions.Content.Edit)]
    public async Task<BlogPostDto> UpdateAsync(Guid id, CreateUpdateBlogPostDto input)
    {
        var entity = await _repository.GetAsync(id);
        var duplicate = await _repository.FindAsync(x => x.Slug == input.Slug && x.Id != id);
        if (duplicate != null)
            throw new Volo.Abp.BusinessException(FitliyoDomainErrorCodes.BlogPostSlugAlreadyExists);

        entity.Title = input.Title;
        entity.Slug = input.Slug;
        entity.Summary = input.Summary;
        entity.Body = input.Body;
        entity.Status = input.Status;
        entity.AuthorName = input.AuthorName;
        entity.FeaturedImageUrl = input.FeaturedImageUrl;
        await _repository.UpdateAsync(entity);
        return ObjectMapper.Map<BlogPost, BlogPostDto>(entity);
    }

    [Authorize(FitliyoPermissions.Content.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    [Authorize(FitliyoPermissions.Content.Edit)]
    public async Task<BlogPostDto> PublishAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        entity.Publish();
        await _repository.UpdateAsync(entity);
        return ObjectMapper.Map<BlogPost, BlogPostDto>(entity);
    }
}
