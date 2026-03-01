using System;
using System.Threading.Tasks;
using Fitliyo.ServicePackages.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.ServicePackages;

public interface IServicePackageAppService : IApplicationService
{
    Task<ServicePackageDto> GetAsync(Guid id);

    Task<PagedResultDto<ServicePackageDto>> GetListAsync(GetPackageListDto input);

    Task<ServicePackageDto> CreateAsync(CreateUpdateServicePackageDto input);

    Task<ServicePackageDto> UpdateAsync(Guid id, CreateUpdateServicePackageDto input);

    Task DeleteAsync(Guid id);
}
