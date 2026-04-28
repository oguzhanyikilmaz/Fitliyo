using System;
using System.Threading.Tasks;
using Fitliyo.Wellness.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Wellness;

public interface IPersonalNutritionPlanAppService : IApplicationService
{
    Task<PersonalNutritionPlanDto> GetAsync(Guid id);
    Task<PagedResultDto<PersonalNutritionPlanDto>> GetListAsync(GetPersonalNutritionPlanListDto input);
    Task<PersonalNutritionPlanDto> CreateAsync(CreateUpdatePersonalNutritionPlanDto input);
    Task<PersonalNutritionPlanDto> UpdateAsync(Guid id, CreateUpdatePersonalNutritionPlanDto input);
    Task DeleteAsync(Guid id);
}
