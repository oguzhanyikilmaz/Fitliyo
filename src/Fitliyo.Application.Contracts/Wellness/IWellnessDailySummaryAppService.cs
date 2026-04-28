using System;
using System.Threading.Tasks;
using Fitliyo.Wellness.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Wellness;

public interface IWellnessDailySummaryAppService : IApplicationService
{
    Task<DailyWellnessSummaryDto> GetDailySummaryAsync(DateTime date);
}
