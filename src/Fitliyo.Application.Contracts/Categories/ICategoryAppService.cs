using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fitliyo.Categories.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Categories;

public interface ICategoryAppService : IApplicationService
{
    Task<CategoryDto> GetAsync(Guid id);

    Task<ListResultDto<CategoryDto>> GetListAsync();

    Task<List<CategoryDto>> GetListByParentAsync(Guid? parentId);

    Task<CategoryDto> CreateAsync(CreateUpdateCategoryDto input);

    Task<CategoryDto> UpdateAsync(Guid id, CreateUpdateCategoryDto input);

    Task DeleteAsync(Guid id);
}
