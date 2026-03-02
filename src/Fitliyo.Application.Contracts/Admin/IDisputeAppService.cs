using System;
using System.Threading.Tasks;
using Fitliyo.Admin.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Admin;

public interface IDisputeAppService : IApplicationService
{
    Task<DisputeDto> CreateAsync(CreateDisputeDto input);
    Task<DisputeDto> GetAsync(Guid id);
    Task<PagedResultDto<DisputeDto>> GetMyDisputesAsync(GetDisputeListDto input);
    Task<PagedResultDto<DisputeDto>> GetListAsync(GetDisputeListDto input);
    Task<DisputeDto> ResolveAsync(Guid id, ResolveDisputeDto input);
}
