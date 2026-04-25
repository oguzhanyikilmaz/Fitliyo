using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Categories.Dtos;
using Fitliyo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Categories;

public class CategoryAppService : FitliyoAppService, ICategoryAppService
{
    private readonly IRepository<Category, Guid> _categoryRepository;

    public CategoryAppService(
        IRepository<Category, Guid> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [AllowAnonymous]
    public async Task<CategoryDto> GetAsync(Guid id)
    {
        var entity = await _categoryRepository.GetAsync(id);
        await EnrichParentCategoryNamesAsync([entity]);
        return MapCategoryToDto(entity);
    }

    [AllowAnonymous]
    public async Task<ListResultDto<CategoryDto>> GetListAsync()
    {
        var entities = await _categoryRepository.GetListAsync(x => x.IsActive);
        var sorted = entities.OrderBy(x => x.SortOrder).ThenBy(x => x.Name);
        var sortedList = sorted.ToList();
        await EnrichParentCategoryNamesAsync(sortedList);
        var dtos = sortedList.Select(MapCategoryToDto).ToList();
        return new ListResultDto<CategoryDto>(dtos);
    }

    [AllowAnonymous]
    public async Task<List<CategoryDto>> GetListByParentAsync(Guid? parentId)
    {
        var entities = await _categoryRepository.GetListAsync(x => x.ParentId == parentId && x.IsActive);
        var sorted = entities.OrderBy(x => x.SortOrder)
                             .ThenBy(x => x.Name)
                             .ToList();
        await EnrichParentCategoryNamesAsync(sorted);
        return sorted.Select(MapCategoryToDto).ToList();
    }

    [Authorize(FitliyoPermissions.Categories.Create)]
    public async Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
    {
        var slugExists = await _categoryRepository.AnyAsync(x => x.Slug == input.Slug);
        if (slugExists)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.CategorySlugAlreadyExists);
        }

        var entity = new Category(GuidGenerator.Create(), input.Name, input.Slug);
        entity.ParentId = input.ParentId;
        entity.IconUrl = input.IconUrl;
        entity.Description = input.Description;
        entity.SortOrder = input.SortOrder;
        await SetParentCategoryNameAsync(entity);

        await _categoryRepository.InsertAsync(entity);
        Logger.LogInformation("Kategori oluşturuldu: {CategoryId}, {Name}", entity.Id, entity.Name);

        return MapCategoryToDto(entity);
    }

    [Authorize(FitliyoPermissions.Categories.Edit)]
    public async Task<CategoryDto> UpdateAsync(Guid id, CreateUpdateCategoryDto input)
    {
        var entity = await _categoryRepository.GetAsync(id);

        if (entity.Slug != input.Slug)
        {
            var slugExists = await _categoryRepository.AnyAsync(x => x.Slug == input.Slug && x.Id != id);
            if (slugExists)
            {
                throw new BusinessException(FitliyoDomainErrorCodes.CategorySlugAlreadyExists);
            }
            entity.SetSlug(input.Slug);
        }

        entity.Name = input.Name;
        entity.ParentId = input.ParentId;
        entity.IconUrl = input.IconUrl;
        entity.Description = input.Description;
        entity.SortOrder = input.SortOrder;
        await SetParentCategoryNameAsync(entity);

        await _categoryRepository.UpdateAsync(entity);
        Logger.LogInformation("Kategori güncellendi: {CategoryId}", entity.Id);

        return MapCategoryToDto(entity);
    }

    [Authorize(FitliyoPermissions.Categories.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _categoryRepository.GetAsync(id);
        await _categoryRepository.DeleteAsync(entity);
        Logger.LogInformation("Kategori silindi: {CategoryId}", id);
    }

    private async Task SetParentCategoryNameAsync(Category category)
    {
        if (!category.ParentId.HasValue)
        {
            category.SetProperty("ParentCategoryName", null);
            return;
        }

        var parentCategory = await _categoryRepository.FindAsync(category.ParentId.Value);
        category.SetProperty("ParentCategoryName", parentCategory?.Name);
    }

    private async Task EnrichParentCategoryNamesAsync(IReadOnlyList<Category> categories)
    {
        if (categories.Count == 0)
        {
            return;
        }

        var parentIds = categories
            .Where(x => x.ParentId.HasValue)
            .Select(x => x.ParentId!.Value)
            .Distinct()
            .ToList();

        if (parentIds.Count == 0)
        {
            return;
        }

        var queryable = await _categoryRepository.GetQueryableAsync();
        var parentCategories = await AsyncExecuter.ToListAsync(queryable.Where(x => parentIds.Contains(x.Id)));
        var parentNameMap = parentCategories.ToDictionary(x => x.Id, x => x.Name);

        var dirtyCategories = new List<Category>();
        foreach (var category in categories)
        {
            if (!category.ParentId.HasValue)
            {
                continue;
            }

            var existingParentName = category.GetProperty<string>("ParentCategoryName");
            if (!string.IsNullOrWhiteSpace(existingParentName))
            {
                continue;
            }

            if (parentNameMap.TryGetValue(category.ParentId.Value, out var parentName) && !string.IsNullOrWhiteSpace(parentName))
            {
                category.SetProperty("ParentCategoryName", parentName);
                dirtyCategories.Add(category);
            }
        }

        if (dirtyCategories.Count > 0)
        {
            await _categoryRepository.UpdateManyAsync(dirtyCategories);
        }
    }

    private CategoryDto MapCategoryToDto(Category category)
    {
        var dto = ObjectMapper.Map<Category, CategoryDto>(category);
        dto.ParentCategoryName = category.GetProperty<string>("ParentCategoryName");
        return dto;
    }
}
