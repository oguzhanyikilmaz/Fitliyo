using System;
using System.Threading.Tasks;
using Fitliyo;
using Fitliyo.Wellness.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Wellness;

[Authorize]
public class UserNotificationPreferencesAppService : FitliyoAppService, IUserNotificationPreferencesAppService
{
    private readonly IRepository<UserNotificationPreferences, Guid> _repository;

    public UserNotificationPreferencesAppService(IRepository<UserNotificationPreferences, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<UserNotificationPreferencesDto> GetMyAsync()
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var row = await _repository.FirstOrDefaultAsync(x => x.UserId == userId);
        if (row == null)
        {
            return new UserNotificationPreferencesDto
            {
                UserId = userId,
                EmailOrdersAndSessions = true,
                EmailMarketing = false,
                PushChat = true,
                PushOrderSession = true,
                PushWellnessReminders = true,
                InAppAll = true
            };
        }

        return ObjectMapper.Map<UserNotificationPreferences, UserNotificationPreferencesDto>(row);
    }

    public async Task<UserNotificationPreferencesDto> UpdateMyAsync(UpdateUserNotificationPreferencesDto input)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var row = await _repository.FirstOrDefaultAsync(x => x.UserId == userId);
        if (row == null)
        {
            row = new UserNotificationPreferences(GuidGenerator.Create(), userId);
            await _repository.InsertAsync(row, autoSave: true);
        }

        row.EmailOrdersAndSessions = input.EmailOrdersAndSessions;
        row.EmailMarketing = input.EmailMarketing;
        row.PushChat = input.PushChat;
        row.PushOrderSession = input.PushOrderSession;
        row.PushWellnessReminders = input.PushWellnessReminders;
        row.InAppAll = input.InAppAll;

        await _repository.UpdateAsync(row);

        Logger.LogInformation("Bildirim tercihleri güncellendi: {UserId}", userId);
        return ObjectMapper.Map<UserNotificationPreferences, UserNotificationPreferencesDto>(row);
    }
}
