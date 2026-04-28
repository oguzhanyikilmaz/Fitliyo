using System;
using System.Threading.Tasks;
using Fitliyo.Wellness.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Wellness;

public interface IUserWorkoutLogAppService : IApplicationService
{
    Task<UserWorkoutLogDto> GetAsync(Guid id);
    Task<PagedResultDto<UserWorkoutLogDto>> GetListAsync(GetUserWorkoutLogListDto input);
    Task<UserWorkoutLogDto> CreateAsync(CreateUserWorkoutLogDto input);
    Task DeleteAsync(Guid id);
}
