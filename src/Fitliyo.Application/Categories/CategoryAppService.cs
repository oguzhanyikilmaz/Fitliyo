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
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Categories;

public class CategoryAppService : FitliyoAppService, ICategoryAppService
{
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly FitliyoApplicationMappers _mapper;

    public CategoryAppService(
        IRepository<Category, Guid> categoryRepository,
        FitliyoApplicationMappers mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<CategoryDto> GetAsync(Guid id)
    {
        var entity = await _categoryRepository.GetAsync(id);
        return _mapper.CategoryToDto(entity);
    }

    [AllowAnonymous]
    public async Task<ListResultDto<CategoryDto>> GetListAsync()
    {
        var entities = await _categoryRepository.GetListAsync(x => x.IsActive);
        var sorted = entities.OrderBy(x => x.SortOrder).ThenBy(x => x.Name);
        var dtos = sorted.Select(_mapper.CategoryToDto).ToList();
        return new ListResultDto<CategoryDto>(dtos);
    }

    [AllowAnonymous]
    public async Task<List<CategoryDto>> GetListByParentAsync(Guid? parentId)
    {
        var entities = await _categoryRepository.GetListAsync(x => x.ParentId == parentId && x.IsActive);
        return entities.OrderBy(x => x.SortOrder)
                       .ThenBy(x => x.Name)
                       .Select(_mapper.CategoryToDto)
                       .ToList();
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

        await _categoryRepository.InsertAsync(entity);
        Logger.LogInformation("Kategori oluşturuldu: {CategoryId}, {Name}", entity.Id, entity.Name);

        return _mapper.CategoryToDto(entity);
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

        await _categoryRepository.UpdateAsync(entity);
        Logger.LogInformation("Kategori güncellendi: {CategoryId}", entity.Id);

        return _mapper.CategoryToDto(entity);
    }

    [Authorize(FitliyoPermissions.Categories.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _categoryRepository.GetAsync(id);
        await _categoryRepository.DeleteAsync(entity);
        Logger.LogInformation("Kategori silindi: {CategoryId}", id);
    }
}
