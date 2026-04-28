using System;
using System.Threading.Tasks;
using Fitliyo.Wellness.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Wellness;

public interface IPersonalWorkoutProgramAppService : IApplicationService
{
    Task<PersonalWorkoutProgramDto> GetAsync(Guid id);
    Task<PagedResultDto<PersonalWorkoutProgramDto>> GetListAsync(GetPersonalWorkoutProgramListDto input);
    Task<PersonalWorkoutProgramDto> CreateAsync(CreateUpdatePersonalWorkoutProgramDto input);
    Task<PersonalWorkoutProgramDto> UpdateAsync(Guid id, CreateUpdatePersonalWorkoutProgramDto input);
    Task DeleteAsync(Guid id);
}
