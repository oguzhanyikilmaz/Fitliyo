using System;
using System.Threading.Tasks;
using Fitliyo.Trainers;
using Fitliyo.Trainers.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/trainer-profiles")]
public class TrainerProfileController : FitliyoController, ITrainerProfileAppService
{
    private readonly ITrainerProfileAppService _trainerProfileAppService;

    public TrainerProfileController(ITrainerProfileAppService trainerProfileAppService)
    {
        _trainerProfileAppService = trainerProfileAppService;
    }

    [HttpGet("{id}")]
    public Task<TrainerProfileDto> GetAsync(Guid id)
    {
        return _trainerProfileAppService.GetAsync(id);
    }

    [HttpGet("by-slug/{slug}")]
    public Task<TrainerProfileDto> GetBySlugAsync(string slug)
    {
        return _trainerProfileAppService.GetBySlugAsync(slug);
    }

    [HttpGet]
    public Task<PagedResultDto<TrainerProfileDto>> GetListAsync([FromQuery] GetTrainerListDto input)
    {
        return _trainerProfileAppService.GetListAsync(input);
    }

    [HttpPost]
    public Task<TrainerProfileDto> CreateAsync(CreateUpdateTrainerProfileDto input)
    {
        return _trainerProfileAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<TrainerProfileDto> UpdateAsync(Guid id, CreateUpdateTrainerProfileDto input)
    {
        return _trainerProfileAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _trainerProfileAppService.DeleteAsync(id);
    }

    [HttpGet("my-profile")]
    public Task<TrainerProfileDto> GetMyProfileAsync()
    {
        return _trainerProfileAppService.GetMyProfileAsync();
    }
}
