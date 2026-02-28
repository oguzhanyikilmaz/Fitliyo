using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fitliyo.Categories;
using Fitliyo.Categories.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/categories")]
public class CategoryController : FitliyoController, ICategoryAppService
{
    private readonly ICategoryAppService _categoryAppService;

    public CategoryController(ICategoryAppService categoryAppService)
    {
        _categoryAppService = categoryAppService;
    }

    [HttpGet("{id}")]
    public Task<CategoryDto> GetAsync(Guid id)
    {
        return _categoryAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<ListResultDto<CategoryDto>> GetListAsync()
    {
        return _categoryAppService.GetListAsync();
    }

    [HttpGet("by-parent")]
    public Task<List<CategoryDto>> GetListByParentAsync([FromQuery] Guid? parentId)
    {
        return _categoryAppService.GetListByParentAsync(parentId);
    }

    [HttpPost]
    public Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input)
    {
        return _categoryAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<CategoryDto> UpdateAsync(Guid id, CreateUpdateCategoryDto input)
    {
        return _categoryAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _categoryAppService.DeleteAsync(id);
    }
}
