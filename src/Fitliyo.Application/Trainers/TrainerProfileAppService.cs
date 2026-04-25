using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Permissions;
using Fitliyo.Trainers.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Fitliyo.Trainers;

[Authorize]
public class TrainerProfileAppService : FitliyoAppService, ITrainerProfileAppService
{
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<TrainerCertificate, Guid> _certificateRepository;
    private readonly IRepository<TrainerGallery, Guid> _galleryRepository;
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

    public TrainerProfileAppService(
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<TrainerCertificate, Guid> certificateRepository,
        IRepository<TrainerGallery, Guid> galleryRepository,
        IRepository<IdentityUser, Guid> identityUserRepository)
    {
        _trainerProfileRepository = trainerProfileRepository;
        _certificateRepository = certificateRepository;
        _galleryRepository = galleryRepository;
        _identityUserRepository = identityUserRepository;
    }

    [AllowAnonymous]
    public async Task<TrainerProfileDto> GetAsync(Guid id)
    {
        var entity = await _trainerProfileRepository.GetAsync(id);
        await EnrichTrainerDisplayNamesAsync([entity]);
        return MapTrainerToDto(entity);
    }

    [AllowAnonymous]
    public async Task<TrainerProfileDto> GetBySlugAsync(string slug)
    {
        var entity = await _trainerProfileRepository.FindAsync(x => x.Slug == slug);
        if (entity == null)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);
        }
        await EnrichTrainerDisplayNamesAsync([entity]);
        return MapTrainerToDto(entity);
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<TrainerProfileDto>> GetListAsync(GetTrainerListDto input)
    {
        var queryable = await _trainerProfileRepository.GetQueryableAsync();

        queryable = queryable.Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x =>
                x.Bio!.Contains(input.Filter) ||
                x.City!.Contains(input.Filter) ||
                x.Slug.Contains(input.Filter));
        }

        if (input.TrainerType.HasValue)
        {
            queryable = queryable.Where(x => x.TrainerType == input.TrainerType.Value);
        }

        if (!string.IsNullOrWhiteSpace(input.City))
        {
            queryable = queryable.Where(x => x.City == input.City);
        }

        if (input.IsOnlineAvailable.HasValue)
        {
            queryable = queryable.Where(x => x.IsOnlineAvailable == input.IsOnlineAvailable.Value);
        }

        if (input.IsOnSiteAvailable.HasValue)
        {
            queryable = queryable.Where(x => x.IsOnSiteAvailable == input.IsOnSiteAvailable.Value);
        }

        if (input.IsVerified.HasValue)
        {
            queryable = queryable.Where(x => x.IsVerified == input.IsVerified.Value);
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        if (!string.IsNullOrWhiteSpace(input.Sorting))
        {
            queryable = queryable.OrderBy(input.Sorting);
        }
        else
        {
            queryable = queryable.OrderByDescending(x => x.AverageRating)
                                 .ThenByDescending(x => x.TotalReviewCount);
        }

        queryable = queryable.PageBy(input);

        var entities = await AsyncExecuter.ToListAsync(queryable);
        await EnrichTrainerDisplayNamesAsync(entities);
        var dtos = entities.Select(MapTrainerToDto).ToList();

        return new PagedResultDto<TrainerProfileDto>(totalCount, dtos);
    }

    [Authorize(FitliyoPermissions.Trainers.Create)]
    public async Task<TrainerProfileDto> CreateAsync(CreateUpdateTrainerProfileDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);

        var existingProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (existingProfile != null)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileAlreadyExists);
        }

        var slugExists = await _trainerProfileRepository.AnyAsync(x => x.Slug == input.Slug);
        if (slugExists)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerSlugAlreadyExists);
        }

        var entity = new TrainerProfile(GuidGenerator.Create(), userId, input.Slug, input.TrainerType);
        ApplyDtoToEntity(input, entity);
        await PopulateTrainerDisplayNameAsync(entity);

        await _trainerProfileRepository.InsertAsync(entity);
        Logger.LogInformation("Eğitmen profili oluşturuldu: {TrainerProfileId}, {UserId}", entity.Id, userId);

        return MapTrainerToDto(entity);
    }

    [Authorize(FitliyoPermissions.Trainers.Edit)]
    public async Task<TrainerProfileDto> UpdateAsync(Guid id, CreateUpdateTrainerProfileDto input)
    {
        var entity = await _trainerProfileRepository.GetAsync(id);
        await CheckOwnershipAsync(entity);

        if (entity.Slug != input.Slug)
        {
            var slugExists = await _trainerProfileRepository.AnyAsync(x => x.Slug == input.Slug && x.Id != id);
            if (slugExists)
            {
                throw new BusinessException(FitliyoDomainErrorCodes.TrainerSlugAlreadyExists);
            }
            entity.SetSlug(input.Slug);
        }

        ApplyDtoToEntity(input, entity);
        await PopulateTrainerDisplayNameAsync(entity);

        await _trainerProfileRepository.UpdateAsync(entity);
        Logger.LogInformation("Eğitmen profili güncellendi: {TrainerProfileId}", entity.Id);

        return MapTrainerToDto(entity);
    }

    [Authorize(FitliyoPermissions.Trainers.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _trainerProfileRepository.GetAsync(id);
        await CheckOwnershipAsync(entity);

        await _trainerProfileRepository.DeleteAsync(entity);
        Logger.LogInformation("Eğitmen profili silindi: {TrainerProfileId}", id);
    }

    [Authorize]
    public async Task<TrainerProfileDto> GetMyProfileAsync()
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var entity = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (entity == null)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);
        }
        await EnrichTrainerDisplayNamesAsync([entity]);
        return MapTrainerToDto(entity);
    }

    private async Task CheckOwnershipAsync(TrainerProfile entity)
    {
        if (entity.UserId != (CurrentUser.Id ?? Guid.Empty))
        {
            var isAdmin = await AuthorizationService.IsGrantedAsync(FitliyoPermissions.Trainers.Verify);
            if (!isAdmin)
            {
                throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);
            }
        }
    }

    private static void ApplyDtoToEntity(CreateUpdateTrainerProfileDto input, TrainerProfile entity)
    {
        entity.Bio = input.Bio;
        entity.ExperienceYears = input.ExperienceYears;
        entity.TrainerType = input.TrainerType;
        entity.SpecialtyTags = input.SpecialtyTags;
        entity.City = input.City;
        entity.District = input.District;
        entity.IsOnlineAvailable = input.IsOnlineAvailable;
        entity.IsOnSiteAvailable = input.IsOnSiteAvailable;
        entity.InstagramUrl = input.InstagramUrl;
        entity.YoutubeUrl = input.YoutubeUrl;
        entity.WebsiteUrl = input.WebsiteUrl;
    }

    private async Task PopulateTrainerDisplayNameAsync(TrainerProfile trainerProfile)
    {
        var user = await _identityUserRepository.FindAsync(trainerProfile.UserId);
        var fullName = BuildFullName(user?.Name, user?.Surname);
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            trainerProfile.SetProperty("TrainerFullName", fullName);
        }
    }

    private async Task EnrichTrainerDisplayNamesAsync(IReadOnlyList<TrainerProfile> profiles)
    {
        if (profiles.Count == 0)
        {
            return;
        }

        var userIds = profiles.Select(x => x.UserId).Distinct().ToList();
        var usersQuery = await _identityUserRepository.GetQueryableAsync();
        var users = await AsyncExecuter.ToListAsync(usersQuery.Where(x => userIds.Contains(x.Id)));
        var userNameMap = users.ToDictionary(x => x.Id, x => BuildFullName(x.Name, x.Surname));

        var dirtyProfiles = new List<TrainerProfile>();
        foreach (var profile in profiles)
        {
            var existingName = profile.GetProperty<string>("TrainerFullName");
            if (!string.IsNullOrWhiteSpace(existingName))
            {
                continue;
            }

            if (userNameMap.TryGetValue(profile.UserId, out var resolvedName) && !string.IsNullOrWhiteSpace(resolvedName))
            {
                profile.SetProperty("TrainerFullName", resolvedName);
                dirtyProfiles.Add(profile);
            }
        }

        if (dirtyProfiles.Count > 0)
        {
            await _trainerProfileRepository.UpdateManyAsync(dirtyProfiles);
        }
    }

    private TrainerProfileDto MapTrainerToDto(TrainerProfile trainerProfile)
    {
        var dto = ObjectMapper.Map<TrainerProfile, TrainerProfileDto>(trainerProfile);
        dto.TrainerFullName = trainerProfile.GetProperty<string>("TrainerFullName");
        return dto;
    }

    private static string? BuildFullName(string? name, string? surname)
    {
        var fullName = $"{name} {surname}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
    }
}
