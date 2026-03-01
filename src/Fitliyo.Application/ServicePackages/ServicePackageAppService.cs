using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Permissions;
using Fitliyo.ServicePackages.Dtos;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.ServicePackages;

[Authorize]
public class ServicePackageAppService : FitliyoAppService, IServicePackageAppService
{
    private readonly IRepository<ServicePackage, Guid> _packageRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;

    public ServicePackageAppService(
        IRepository<ServicePackage, Guid> packageRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository)
    {
        _packageRepository = packageRepository;
        _trainerProfileRepository = trainerProfileRepository;
    }

    [AllowAnonymous]
    public async Task<ServicePackageDto> GetAsync(Guid id)
    {
        var entity = await _packageRepository.GetAsync(id);
        return ObjectMapper.Map<ServicePackage, ServicePackageDto>(entity);
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<ServicePackageDto>> GetListAsync(GetPackageListDto input)
    {
        var queryable = await _packageRepository.GetQueryableAsync();

        queryable = queryable.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x =>
                x.Title.Contains(input.Filter) ||
                x.Description!.Contains(input.Filter));
        }

        if (input.TrainerProfileId.HasValue)
        {
            queryable = queryable.Where(x => x.TrainerProfileId == input.TrainerProfileId.Value);
        }

        if (input.PackageType.HasValue)
        {
            queryable = queryable.Where(x => x.PackageType == input.PackageType.Value);
        }

        if (input.IsOnline.HasValue)
        {
            queryable = queryable.Where(x => x.IsOnline == input.IsOnline.Value);
        }

        if (input.IsOnSite.HasValue)
        {
            queryable = queryable.Where(x => x.IsOnSite == input.IsOnSite.Value);
        }

        if (input.MinPrice.HasValue)
        {
            var effectivePrice = input.MinPrice.Value;
            queryable = queryable.Where(x => (x.DiscountedPrice ?? x.Price) >= effectivePrice);
        }

        if (input.MaxPrice.HasValue)
        {
            var effectivePrice = input.MaxPrice.Value;
            queryable = queryable.Where(x => (x.DiscountedPrice ?? x.Price) <= effectivePrice);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            queryable = queryable.OrderBy(input.Sorting);
        }
        else
        {
            queryable = queryable.OrderByDescending(x => x.IsFeatured)
                                 .ThenByDescending(x => x.AverageRating);
        }

        queryable = queryable.PageBy(input);

        var entities = await AsyncExecuter.ToListAsync(queryable);
        var dtos = entities.Select(x => ObjectMapper.Map<ServicePackage, ServicePackageDto>(x)).ToList();

        return new PagedResultDto<ServicePackageDto>(totalCount, dtos);
    }

    [Authorize(FitliyoPermissions.Packages.Create)]
    public async Task<ServicePackageDto> CreateAsync(CreateUpdateServicePackageDto input)
    {
        var trainerProfile = await GetCurrentTrainerProfileAsync();

        var entity = new ServicePackage(
            GuidGenerator.Create(),
            trainerProfile.Id,
            input.Title,
            input.PackageType,
            input.Price);

        ApplyDtoToEntity(input, entity);

        await _packageRepository.InsertAsync(entity);
        Logger.LogInformation("Hizmet paketi oluşturuldu: {PackageId}, Eğitmen: {TrainerProfileId}", entity.Id, trainerProfile.Id);

        return ObjectMapper.Map<ServicePackage, ServicePackageDto>(entity);
    }

    [Authorize(FitliyoPermissions.Packages.Edit)]
    public async Task<ServicePackageDto> UpdateAsync(Guid id, CreateUpdateServicePackageDto input)
    {
        var entity = await _packageRepository.GetAsync(id);
        await CheckPackageOwnershipAsync(entity);

        entity.Title = input.Title;
        entity.PackageType = input.PackageType;
        entity.Price = input.Price;
        ApplyDtoToEntity(input, entity);

        await _packageRepository.UpdateAsync(entity);
        Logger.LogInformation("Hizmet paketi güncellendi: {PackageId}", entity.Id);

        return ObjectMapper.Map<ServicePackage, ServicePackageDto>(entity);
    }

    [Authorize(FitliyoPermissions.Packages.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _packageRepository.GetAsync(id);
        await CheckPackageOwnershipAsync(entity);

        await _packageRepository.DeleteAsync(entity);
        Logger.LogInformation("Hizmet paketi silindi: {PackageId}", id);
    }

    private async Task<TrainerProfile> GetCurrentTrainerProfileAsync()
    {
        var userId = CurrentUser.GetId();
        var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (trainerProfile == null)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);
        }
        return trainerProfile;
    }

    private async Task CheckPackageOwnershipAsync(ServicePackage package)
    {
        var trainerProfile = await _trainerProfileRepository.GetAsync(package.TrainerProfileId);
        if (trainerProfile.UserId != CurrentUser.GetId())
        {
            var isAdmin = await AuthorizationService.IsGrantedAsync(FitliyoPermissions.Packages.Edit);
            if (!isAdmin)
            {
                throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
            }
        }
    }

    private static void ApplyDtoToEntity(CreateUpdateServicePackageDto input, ServicePackage entity)
    {
        entity.Description = input.Description;
        entity.DiscountedPrice = input.DiscountedPrice;
        entity.Currency = input.Currency;
        entity.DurationDays = input.DurationDays;
        entity.SessionCount = input.SessionCount;
        entity.SessionDurationMinutes = input.SessionDurationMinutes;
        entity.MaxStudents = input.MaxStudents;
        entity.IsOnline = input.IsOnline;
        entity.IsOnSite = input.IsOnSite;
        entity.CancellationHours = input.CancellationHours;
        entity.CancellationPolicy = input.CancellationPolicy;
        entity.WhatIsIncluded = input.WhatIsIncluded;
        entity.WhatIsNotIncluded = input.WhatIsNotIncluded;
        entity.Tags = input.Tags;
    }
}
