using System;
using System.Threading.Tasks;
using Fitliyo.Subscriptions.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Subscriptions;

public interface ISubscriptionAppService : IApplicationService
{
    Task<ListResultDto<SubscriptionPlanDto>> GetPlansAsync();

    Task<SubscriptionPlanDto> CreatePlanAsync(CreateUpdateSubscriptionPlanDto input);

    Task<SubscriptionPlanDto> UpdatePlanAsync(Guid id, CreateUpdateSubscriptionPlanDto input);

    Task DeletePlanAsync(Guid id);

    Task<TrainerSubscriptionDto> GetMySubscriptionAsync();

    Task<TrainerSubscriptionDto> SubscribeAsync(SubscribeDto input);

    Task CancelSubscriptionAsync();
}
