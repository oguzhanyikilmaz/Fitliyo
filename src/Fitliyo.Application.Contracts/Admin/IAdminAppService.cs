using System;
using System.Threading.Tasks;
using Fitliyo.Admin.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Admin;

public interface IAdminAppService : IApplicationService
{
    Task<DashboardDto> GetDashboardAsync();

    Task<PlatformStatsDto> GetPlatformStatsAsync(DateTime startDate, DateTime endDate);

    Task VerifyTrainerAsync(Guid trainerProfileId);

    Task UnverifyTrainerAsync(Guid trainerProfileId);

    Task ToggleReviewVisibilityAsync(Guid reviewId);
}
