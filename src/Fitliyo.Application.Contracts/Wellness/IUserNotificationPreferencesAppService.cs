using System.Threading.Tasks;
using Fitliyo.Wellness.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Wellness;

public interface IUserNotificationPreferencesAppService : IApplicationService
{
    Task<UserNotificationPreferencesDto> GetMyAsync();
    Task<UserNotificationPreferencesDto> UpdateMyAsync(UpdateUserNotificationPreferencesDto input);
}
