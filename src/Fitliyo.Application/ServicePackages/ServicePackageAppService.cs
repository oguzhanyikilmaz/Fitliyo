using System;
using System.Collections.Generic;
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
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Fitliyo.ServicePackages;

[Authorize]
public class ServicePackageAppService : FitliyoAppService, IServicePackageAppService
{
    private readonly IRepository<ServicePackage, Guid> _packageRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

    public ServicePackageAppService(
        IRepository<ServicePackage, Guid> packageRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<IdentityUser, Guid> identityUserRepository)
    {
        _packageRepository = packageRepository;
        _trainerProfileRepository = trainerProfileRepository;
        _identityUserRepository = identityUserRepository;
    }

    [AllowAnonymous]
    public async Task<ServicePackageDto> GetAsync(Guid id)
    {
        var entity = await _packageRepository.GetAsync(id);
        await EnrichPackageDisplayNamesAsync([entity]);
        return MapPackageToDto(entity);
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
        await EnrichPackageDisplayNamesAsync(entities);
        var dtos = entities.Select(MapPackageToDto).ToList();

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
        await PopulatePackageDisplayNameAsync(entity, trainerProfile);

        await _packageRepository.InsertAsync(entity);
        Logger.LogInformation("Hizmet paketi oluşturuldu: {PackageId}, Eğitmen: {TrainerProfileId}", entity.Id, trainerProfile.Id);

        return MapPackageToDto(entity);
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
        var trainerProfile = await _trainerProfileRepository.GetAsync(entity.TrainerProfileId);
        await PopulatePackageDisplayNameAsync(entity, trainerProfile);

        await _packageRepository.UpdateAsync(entity);
        Logger.LogInformation("Hizmet paketi güncellendi: {PackageId}", entity.Id);

        return MapPackageToDto(entity);
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
        var userId = (CurrentUser.Id ?? Guid.Empty);
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
        if (trainerProfile.UserId != (CurrentUser.Id ?? Guid.Empty))
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

    private async Task PopulatePackageDisplayNameAsync(ServicePackage package, TrainerProfile trainerProfile)
    {
        var trainerUser = await _identityUserRepository.FindAsync(trainerProfile.UserId);
        var trainerFullName = BuildFullName(trainerUser?.Name, trainerUser?.Surname);
        if (!string.IsNullOrWhiteSpace(trainerFullName))
        {
            package.SetProperty("TrainerFullName", trainerFullName);
        }
    }

    private async Task EnrichPackageDisplayNamesAsync(IReadOnlyList<ServicePackage> packages)
    {
        if (packages.Count == 0)
        {
            return;
        }

        var trainerProfileIds = packages.Select(x => x.TrainerProfileId).Distinct().ToList();
        var trainerProfilesQuery = await _trainerProfileRepository.GetQueryableAsync();
        var trainerProfiles = await AsyncExecuter.ToListAsync(trainerProfilesQuery.Where(x => trainerProfileIds.Contains(x.Id)));
        var trainerProfileMap = trainerProfiles.ToDictionary(x => x.Id, x => x.UserId);

        var userIds = trainerProfiles.Select(x => x.UserId).Distinct().ToList();
        var usersQuery = await _identityUserRepository.GetQueryableAsync();
        var users = await AsyncExecuter.ToListAsync(usersQuery.Where(x => userIds.Contains(x.Id)));
        var userNameMap = users.ToDictionary(x => x.Id, x => BuildFullName(x.Name, x.Surname));

        var dirtyPackages = new List<ServicePackage>();
        foreach (var package in packages)
        {
            var existingName = package.GetProperty<string>("TrainerFullName");
            if (!string.IsNullOrWhiteSpace(existingName))
            {
                continue;
            }

            if (trainerProfileMap.TryGetValue(package.TrainerProfileId, out var trainerUserId)
                && userNameMap.TryGetValue(trainerUserId, out var resolvedName)
                && !string.IsNullOrWhiteSpace(resolvedName))
            {
                package.SetProperty("TrainerFullName", resolvedName);
                dirtyPackages.Add(package);
            }
        }

        if (dirtyPackages.Count > 0)
        {
            await _packageRepository.UpdateManyAsync(dirtyPackages);
        }
    }

    private ServicePackageDto MapPackageToDto(ServicePackage package)
    {
        var dto = ObjectMapper.Map<ServicePackage, ServicePackageDto>(package);
        dto.TrainerFullName = package.GetProperty<string>("TrainerFullName");
        return dto;
    }

    private static string? BuildFullName(string? name, string? surname)
    {
        var fullName = $"{name} {surname}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
    }
}
