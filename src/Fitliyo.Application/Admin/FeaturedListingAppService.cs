using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Admin.Dtos;
using Fitliyo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Admin;

[Authorize(FitliyoPermissions.Admin.FeaturedListings)]
public class FeaturedListingAppService : FitliyoAppService, IFeaturedListingAppService
{
    private readonly IRepository<FeaturedListing, Guid> _repository;

    public FeaturedListingAppService(IRepository<FeaturedListing, Guid> repository)
    {
        _repository = repository;
    }

    [Authorize]
    public async Task<FeaturedListingDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return ObjectMapper.Map<FeaturedListing, FeaturedListingDto>(entity);
    }

    [Authorize]
    public async Task<PagedResultDto<FeaturedListingDto>> GetListAsync(GetFeaturedListingListDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        if (input.PageType.HasValue) queryable = queryable.Where(x => x.PageType == input.PageType.Value);
        if (input.IsActive.HasValue) queryable = queryable.Where(x => x.IsActive == input.IsActive.Value);
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderBy(x => x.SortOrder);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<FeaturedListingDto>(totalCount, items.Select(x => ObjectMapper.Map<FeaturedListing, FeaturedListingDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Admin.FeaturedListings)]
    public async Task<FeaturedListingDto> CreateAsync(CreateUpdateFeaturedListingDto input)
    {
        var entity = new FeaturedListing(
            GuidGenerator.Create(),
            input.PageType,
            input.SortOrder,
            input.TrainerProfileId,
            input.ServicePackageId);
        entity.StartDate = input.StartDate;
        entity.EndDate = input.EndDate;
        entity.IsActive = input.IsActive;
        entity.AdminNote = input.AdminNote;
        await _repository.InsertAsync(entity);
        return ObjectMapper.Map<FeaturedListing, FeaturedListingDto>(entity);
    }

    [Authorize(FitliyoPermissions.Admin.FeaturedListings)]
    public async Task<FeaturedListingDto> UpdateAsync(Guid id, CreateUpdateFeaturedListingDto input)
    {
        var entity = await _repository.GetAsync(id);
        entity.PageType = input.PageType;
        entity.TrainerProfileId = input.TrainerProfileId;
        entity.ServicePackageId = input.ServicePackageId;
        entity.SortOrder = input.SortOrder;
        entity.StartDate = input.StartDate;
        entity.EndDate = input.EndDate;
        entity.IsActive = input.IsActive;
        entity.AdminNote = input.AdminNote;
        await _repository.UpdateAsync(entity);
        return ObjectMapper.Map<FeaturedListing, FeaturedListingDto>(entity);
    }

    [Authorize(FitliyoPermissions.Admin.FeaturedListings)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
