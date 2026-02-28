using System;
using System.Threading.Tasks;
using Fitliyo.Trainers.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Trainers;

public interface ITrainerProfileAppService : IApplicationService
{
    Task<TrainerProfileDto> GetAsync(Guid id);

    Task<TrainerProfileDto> GetBySlugAsync(string slug);

    Task<PagedResultDto<TrainerProfileDto>> GetListAsync(GetTrainerListDto input);

    Task<TrainerProfileDto> CreateAsync(CreateUpdateTrainerProfileDto input);

    Task<TrainerProfileDto> UpdateAsync(Guid id, CreateUpdateTrainerProfileDto input);

    Task DeleteAsync(Guid id);

    Task<TrainerProfileDto> GetMyProfileAsync();
}
