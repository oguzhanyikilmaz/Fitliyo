using System;
using System.Threading.Tasks;
using Fitliyo.Wellness.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Wellness;

public interface IUserMealLogAppService : IApplicationService
{
    Task<UserDailyMealLogDto> GetOrCreateDailyLogAsync(DateTime logDate);
    Task<UserMealLogEntryDto> AddEntryAsync(Guid dailyLogId, CreateMealLogEntryDto input);
    Task<UserMealLogEntryDto> UpdateEntryAsync(Guid entryId, UpdateMealLogEntryDto input);
    Task RemoveEntryAsync(Guid entryId);
}
