using System;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Permissions;
using Fitliyo.Subscriptions.Dtos;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Subscriptions;

[Authorize]
public class SubscriptionAppService : FitliyoAppService, ISubscriptionAppService
{
    private readonly IRepository<SubscriptionPlan, Guid> _planRepository;
    private readonly IRepository<TrainerSubscription, Guid> _subscriptionRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;

    public SubscriptionAppService(
        IRepository<SubscriptionPlan, Guid> planRepository,
        IRepository<TrainerSubscription, Guid> subscriptionRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository)
    {
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
        _trainerProfileRepository = trainerProfileRepository;
    }

    [AllowAnonymous]
    public async Task<ListResultDto<SubscriptionPlanDto>> GetPlansAsync()
    {
        var plans = await _planRepository.GetListAsync(x => x.IsActive);
        var sorted = plans.OrderBy(x => x.SortOrder).ThenBy(x => x.Price);
        return new ListResultDto<SubscriptionPlanDto>(sorted.Select(x => ObjectMapper.Map<SubscriptionPlan, SubscriptionPlanDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Admin.Dashboard)]
    public async Task<SubscriptionPlanDto> CreatePlanAsync(CreateUpdateSubscriptionPlanDto input)
    {
        var entity = new SubscriptionPlan(
            GuidGenerator.Create(), input.Name, input.Tier, input.Price, input.CommissionRate);

        entity.Description = input.Description;
        entity.PlanType = input.PlanType;
        entity.MaxPackageCount = input.MaxPackageCount;
        entity.HasFeaturedListing = input.HasFeaturedListing;
        entity.HasPrioritySupport = input.HasPrioritySupport;
        entity.HasAdvancedAnalytics = input.HasAdvancedAnalytics;
        entity.FeaturesJson = input.FeaturesJson;
        entity.SortOrder = input.SortOrder;

        await _planRepository.InsertAsync(entity);
        Logger.LogInformation("Abonelik planı oluşturuldu: {PlanId}, {Name}", entity.Id, entity.Name);

        return ObjectMapper.Map<SubscriptionPlan, SubscriptionPlanDto>(entity);
    }

    [Authorize(FitliyoPermissions.Admin.Dashboard)]
    public async Task<SubscriptionPlanDto> UpdatePlanAsync(Guid id, CreateUpdateSubscriptionPlanDto input)
    {
        var entity = await _planRepository.GetAsync(id);

        entity.Name = input.Name;
        entity.Description = input.Description;
        entity.Tier = input.Tier;
        entity.PlanType = input.PlanType;
        entity.Price = input.Price;
        entity.MaxPackageCount = input.MaxPackageCount;
        entity.CommissionRate = input.CommissionRate;
        entity.HasFeaturedListing = input.HasFeaturedListing;
        entity.HasPrioritySupport = input.HasPrioritySupport;
        entity.HasAdvancedAnalytics = input.HasAdvancedAnalytics;
        entity.FeaturesJson = input.FeaturesJson;
        entity.SortOrder = input.SortOrder;

        await _planRepository.UpdateAsync(entity);
        Logger.LogInformation("Abonelik planı güncellendi: {PlanId}", id);

        return ObjectMapper.Map<SubscriptionPlan, SubscriptionPlanDto>(entity);
    }

    [Authorize(FitliyoPermissions.Admin.Dashboard)]
    public async Task DeletePlanAsync(Guid id)
    {
        await _planRepository.DeleteAsync(id);
        Logger.LogInformation("Abonelik planı silindi: {PlanId}", id);
    }

    [Authorize]
    public async Task<TrainerSubscriptionDto> GetMySubscriptionAsync()
    {
        var trainerProfile = await GetCurrentTrainerProfileAsync();

        var subscription = await _subscriptionRepository.FindAsync(
            x => x.TrainerProfileId == trainerProfile.Id &&
                 (x.Status == SubscriptionStatus.Active || x.Status == SubscriptionStatus.Trial));

        if (subscription == null)
            throw new BusinessException(FitliyoDomainErrorCodes.EntityNotFound);

        return ObjectMapper.Map<TrainerSubscription, TrainerSubscriptionDto>(subscription);
    }

    [Authorize]
    public async Task<TrainerSubscriptionDto> SubscribeAsync(SubscribeDto input)
    {
        var trainerProfile = await GetCurrentTrainerProfileAsync();
        var plan = await _planRepository.GetAsync(input.SubscriptionPlanId);

        var existingActive = await _subscriptionRepository.FindAsync(
            x => x.TrainerProfileId == trainerProfile.Id && x.Status == SubscriptionStatus.Active);

        if (existingActive != null)
        {
            existingActive.Cancel();
            await _subscriptionRepository.UpdateAsync(existingActive);
        }

        var monthsToAdd = (int)plan.PlanType;
        var startDate = DateTime.Now;
        var endDate = startDate.AddMonths(monthsToAdd);

        var subscription = new TrainerSubscription(
            GuidGenerator.Create(), trainerProfile.Id, plan.Id, startDate, endDate, plan.Price);
        subscription.IsAutoRenew = input.IsAutoRenew;

        await _subscriptionRepository.InsertAsync(subscription);

        trainerProfile.SubscriptionTier = plan.Tier;
        trainerProfile.SubscriptionExpiry = endDate;
        await _trainerProfileRepository.UpdateAsync(trainerProfile);

        Logger.LogInformation("Abonelik oluşturuldu: {SubscriptionId}, Eğitmen: {TrainerProfileId}, Plan: {PlanName}",
            subscription.Id, trainerProfile.Id, plan.Name);

        return ObjectMapper.Map<TrainerSubscription, TrainerSubscriptionDto>(subscription);
    }

    [Authorize]
    public async Task CancelSubscriptionAsync()
    {
        var trainerProfile = await GetCurrentTrainerProfileAsync();

        var subscription = await _subscriptionRepository.FindAsync(
            x => x.TrainerProfileId == trainerProfile.Id && x.Status == SubscriptionStatus.Active);

        if (subscription == null)
            throw new BusinessException(FitliyoDomainErrorCodes.EntityNotFound);

        subscription.Cancel();
        await _subscriptionRepository.UpdateAsync(subscription);

        trainerProfile.SubscriptionTier = SubscriptionTier.Free;
        trainerProfile.SubscriptionExpiry = null;
        await _trainerProfileRepository.UpdateAsync(trainerProfile);

        Logger.LogInformation("Abonelik iptal edildi: {SubscriptionId}", subscription.Id);
    }

    private async Task<TrainerProfile> GetCurrentTrainerProfileAsync()
    {
        var userId = CurrentUser.GetId();
        var trainerProfile = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (trainerProfile == null)
            throw new BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);
        return trainerProfile;
    }
}
