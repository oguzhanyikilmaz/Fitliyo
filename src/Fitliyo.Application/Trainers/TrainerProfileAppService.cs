using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Permissions;
using Fitliyo.Trainers.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Trainers;

[Authorize]
public class TrainerProfileAppService : FitliyoAppService, ITrainerProfileAppService
{
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<TrainerCertificate, Guid> _certificateRepository;
    private readonly IRepository<TrainerGallery, Guid> _galleryRepository;

    public TrainerProfileAppService(
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<TrainerCertificate, Guid> certificateRepository,
        IRepository<TrainerGallery, Guid> galleryRepository)
    {
        _trainerProfileRepository = trainerProfileRepository;
        _certificateRepository = certificateRepository;
        _galleryRepository = galleryRepository;
    }

    [AllowAnonymous]
    public async Task<TrainerProfileDto> GetAsync(Guid id)
    {
        var entity = await _trainerProfileRepository.GetAsync(id);
        return ObjectMapper.Map<TrainerProfile, TrainerProfileDto>(entity);
    }

    [AllowAnonymous]
    public async Task<TrainerProfileDto> GetBySlugAsync(string slug)
    {
        var entity = await _trainerProfileRepository.FindAsync(x => x.Slug == slug);
        if (entity == null)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);
        }
        return ObjectMapper.Map<TrainerProfile, TrainerProfileDto>(entity);
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
        var dtos = entities.Select(x => ObjectMapper.Map<TrainerProfile, TrainerProfileDto>(x)).ToList();

        return new PagedResultDto<TrainerProfileDto>(totalCount, dtos);
    }

    [Authorize(FitliyoPermissions.Trainers.Create)]
    public async Task<TrainerProfileDto> CreateAsync(CreateUpdateTrainerProfileDto input)
    {
        var userId = CurrentUser.GetId();

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

        await _trainerProfileRepository.InsertAsync(entity);
        Logger.LogInformation("Eğitmen profili oluşturuldu: {TrainerProfileId}, {UserId}", entity.Id, userId);

        return ObjectMapper.Map<TrainerProfile, TrainerProfileDto>(entity);
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

        await _trainerProfileRepository.UpdateAsync(entity);
        Logger.LogInformation("Eğitmen profili güncellendi: {TrainerProfileId}", entity.Id);

        return ObjectMapper.Map<TrainerProfile, TrainerProfileDto>(entity);
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
        var userId = CurrentUser.GetId();
        var entity = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (entity == null)
        {
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);
        }
        return ObjectMapper.Map<TrainerProfile, TrainerProfileDto>(entity);
    }

    private async Task CheckOwnershipAsync(TrainerProfile entity)
    {
        if (entity.UserId != CurrentUser.GetId())
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
}
