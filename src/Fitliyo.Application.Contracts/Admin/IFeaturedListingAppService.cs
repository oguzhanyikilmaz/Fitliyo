using System;
using System.Threading.Tasks;
using Fitliyo.Admin.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Admin;

public interface IFeaturedListingAppService : IApplicationService
{
    Task<FeaturedListingDto> GetAsync(Guid id);
    Task<PagedResultDto<FeaturedListingDto>> GetListAsync(GetFeaturedListingListDto input);
    Task<FeaturedListingDto> CreateAsync(CreateUpdateFeaturedListingDto input);
    Task<FeaturedListingDto> UpdateAsync(Guid id, CreateUpdateFeaturedListingDto input);
    Task DeleteAsync(Guid id);
}
