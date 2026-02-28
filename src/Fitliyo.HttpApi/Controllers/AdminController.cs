using System;
using System.Threading.Tasks;
using Fitliyo.Admin;
using Fitliyo.Admin.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/admin")]
public class AdminController : FitliyoController, IAdminAppService
{
    private readonly IAdminAppService _adminAppService;

    public AdminController(IAdminAppService adminAppService)
    {
        _adminAppService = adminAppService;
    }

    [HttpGet("dashboard")]
    public Task<DashboardDto> GetDashboardAsync()
    {
        return _adminAppService.GetDashboardAsync();
    }

    [HttpGet("stats")]
    public Task<PlatformStatsDto> GetPlatformStatsAsync([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        return _adminAppService.GetPlatformStatsAsync(startDate, endDate);
    }

    [HttpPost("trainers/{trainerProfileId}/verify")]
    public Task VerifyTrainerAsync(Guid trainerProfileId)
    {
        return _adminAppService.VerifyTrainerAsync(trainerProfileId);
    }

    [HttpPost("trainers/{trainerProfileId}/unverify")]
    public Task UnverifyTrainerAsync(Guid trainerProfileId)
    {
        return _adminAppService.UnverifyTrainerAsync(trainerProfileId);
    }

    [HttpPost("reviews/{reviewId}/toggle-visibility")]
    public Task ToggleReviewVisibilityAsync(Guid reviewId)
    {
        return _adminAppService.ToggleReviewVisibilityAsync(reviewId);
    }
}
