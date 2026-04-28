using System;
using System.Threading.Tasks;
using Fitliyo.Wellness.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Wellness;

public interface IUserFoodItemAppService : IApplicationService
{
    Task<UserFoodItemDto> GetAsync(Guid id);
    Task<PagedResultDto<UserFoodItemDto>> GetListAsync(GetUserFoodItemListDto input);
    Task<UserFoodItemDto> CreateAsync(CreateUpdateUserFoodItemDto input);
    Task<UserFoodItemDto> UpdateAsync(Guid id, CreateUpdateUserFoodItemDto input);
    Task DeleteAsync(Guid id);
}
