using System;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo;
using Fitliyo.Profiles;
using Fitliyo.Wellness.Dtos;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Wellness;

[Authorize]
public class WellnessDailySummaryAppService : FitliyoAppService, IWellnessDailySummaryAppService
{
    private readonly IRepository<UserDailyMealLog, Guid> _mealDailyRepository;
    private readonly IRepository<UserMealLogEntry, Guid> _mealEntryRepository;
    private readonly IRepository<UserWorkoutLog, Guid> _workoutLogRepository;
    private readonly IRepository<PersonalNutritionPlan, Guid> _nutritionRepository;
    private readonly IUserProfileAppService _userProfileAppService;

    public WellnessDailySummaryAppService(
        IRepository<UserDailyMealLog, Guid> mealDailyRepository,
        IRepository<UserMealLogEntry, Guid> mealEntryRepository,
        IRepository<UserWorkoutLog, Guid> workoutLogRepository,
        IRepository<PersonalNutritionPlan, Guid> nutritionRepository,
        IUserProfileAppService userProfileAppService)
    {
        _mealDailyRepository = mealDailyRepository;
        _mealEntryRepository = mealEntryRepository;
        _workoutLogRepository = workoutLogRepository;
        _nutritionRepository = nutritionRepository;
        _userProfileAppService = userProfileAppService;
    }

    public async Task<DailyWellnessSummaryDto> GetDailySummaryAsync(DateTime date)
    {
        var userId = CurrentUser.Id ?? throw new BusinessException(FitliyoDomainErrorCodes.UserNotLoggedIn);
        var day = date.Date;
        var profile = await _userProfileAppService.GetMyProfileAsync();

        var dayLog = await _mealDailyRepository.FindAsync(x => x.UserId == userId && x.LogDate == day);
        decimal intake = 0, pTot = 0, cTot = 0, fTot = 0;
        if (dayLog != null)
        {
            var es = await _mealEntryRepository.GetListAsync(x => x.UserDailyMealLogId == dayLog.Id);
            intake = es.Sum(x => x.Calories);
            pTot = es.Sum(x => x.ProteinG);
            cTot = es.Sum(x => x.CarbsG);
            fTot = es.Sum(x => x.FatG);
        }

        var wLogs = await _workoutLogRepository.GetListAsync(x => x.UserId == userId && x.LogDate == day);
        var burn = wLogs.Sum(x => x.TotalCaloriesBurned);

        var activePlan = await _nutritionRepository.FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);
        var target = activePlan?.DailyCalorieTarget ?? profile.Tdee;
        var refTitle = activePlan?.Title;
        var remaining = target.HasValue
            ? target.Value - intake + burn
            : (decimal?)null;

        return new DailyWellnessSummaryDto
        {
            Date = day,
            TotalCaloriesConsumed = intake,
            TotalProteinG = pTot,
            TotalCarbsG = cTot,
            TotalFatG = fTot,
            TotalCaloriesBurnedFromWorkouts = burn,
            Tdee = profile.Tdee,
            Bmr = profile.Bmr,
            ActiveNutritionPlanTitle = refTitle,
            ReferenceCalorieTarget = target,
            RemainingCalorieBudget = remaining
        };
    }
}
