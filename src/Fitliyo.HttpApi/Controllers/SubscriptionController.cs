using System;
using System.Threading.Tasks;
using Fitliyo.Subscriptions;
using Fitliyo.Subscriptions.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/subscriptions")]
public class SubscriptionController : FitliyoController, ISubscriptionAppService
{
    private readonly ISubscriptionAppService _subscriptionAppService;

    public SubscriptionController(ISubscriptionAppService subscriptionAppService)
    {
        _subscriptionAppService = subscriptionAppService;
    }

    [HttpGet("plans")]
    public Task<ListResultDto<SubscriptionPlanDto>> GetPlansAsync()
    {
        return _subscriptionAppService.GetPlansAsync();
    }

    [HttpPost("plans")]
    public Task<SubscriptionPlanDto> CreatePlanAsync(CreateUpdateSubscriptionPlanDto input)
    {
        return _subscriptionAppService.CreatePlanAsync(input);
    }

    [HttpPut("plans/{id}")]
    public Task<SubscriptionPlanDto> UpdatePlanAsync(Guid id, CreateUpdateSubscriptionPlanDto input)
    {
        return _subscriptionAppService.UpdatePlanAsync(id, input);
    }

    [HttpDelete("plans/{id}")]
    public Task DeletePlanAsync(Guid id)
    {
        return _subscriptionAppService.DeletePlanAsync(id);
    }

    [HttpGet("my-subscription")]
    public Task<TrainerSubscriptionDto> GetMySubscriptionAsync()
    {
        return _subscriptionAppService.GetMySubscriptionAsync();
    }

    [HttpPost("subscribe")]
    public Task<TrainerSubscriptionDto> SubscribeAsync(SubscribeDto input)
    {
        return _subscriptionAppService.SubscribeAsync(input);
    }

    [HttpPost("cancel")]
    public Task CancelSubscriptionAsync()
    {
        return _subscriptionAppService.CancelSubscriptionAsync();
    }
}
