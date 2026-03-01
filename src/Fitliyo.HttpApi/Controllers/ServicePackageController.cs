using System;
using System.Threading.Tasks;
using Fitliyo.ServicePackages;
using Fitliyo.ServicePackages.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/service-packages")]
public class ServicePackageController : FitliyoController, IServicePackageAppService
{
    private readonly IServicePackageAppService _servicePackageAppService;

    public ServicePackageController(IServicePackageAppService servicePackageAppService)
    {
        _servicePackageAppService = servicePackageAppService;
    }

    [HttpGet("{id}")]
    public Task<ServicePackageDto> GetAsync(Guid id)
    {
        return _servicePackageAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ServicePackageDto>> GetListAsync([FromQuery] GetPackageListDto input)
    {
        return _servicePackageAppService.GetListAsync(input);
    }

    [HttpPost]
    public Task<ServicePackageDto> CreateAsync(CreateUpdateServicePackageDto input)
    {
        return _servicePackageAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<ServicePackageDto> UpdateAsync(Guid id, CreateUpdateServicePackageDto input)
    {
        return _servicePackageAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _servicePackageAppService.DeleteAsync(id);
    }
}
