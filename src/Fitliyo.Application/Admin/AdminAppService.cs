using System;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Admin.Dtos;
using Fitliyo.Categories;
using Fitliyo.Enums;
using Fitliyo.Orders;
using Fitliyo.ServicePackages;
using Fitliyo.Permissions;
using Fitliyo.Reviews;
using Fitliyo.Subscriptions;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Admin;

[Authorize(FitliyoPermissions.Admin.Dashboard)]
public class AdminAppService : FitliyoAppService, IAdminAppService
{
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<Review, Guid> _reviewRepository;
    private readonly IRepository<TrainerSubscription, Guid> _subscriptionRepository;
    private readonly IRepository<ServicePackage, Guid> _packageRepository;
    private readonly IRepository<Category, Guid> _categoryRepository;

    public AdminAppService(
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<Order, Guid> orderRepository,
        IRepository<Review, Guid> reviewRepository,
        IRepository<TrainerSubscription, Guid> subscriptionRepository,
        IRepository<ServicePackage, Guid> packageRepository,
        IRepository<Category, Guid> categoryRepository)
    {
        _trainerProfileRepository = trainerProfileRepository;
        _orderRepository = orderRepository;
        _reviewRepository = reviewRepository;
        _subscriptionRepository = subscriptionRepository;
        _packageRepository = packageRepository;
        _categoryRepository = categoryRepository;
    }

    [Authorize(FitliyoPermissions.Admin.Dashboard)]
    public async Task<DashboardDto> GetDashboardAsync()
    {
        var trainerQuery = await _trainerProfileRepository.GetQueryableAsync();
        var orderQuery = await _orderRepository.GetQueryableAsync();
        var reviewQuery = await _reviewRepository.GetQueryableAsync();
        var subscriptionQuery = await _subscriptionRepository.GetQueryableAsync();
        var packageQuery = await _packageRepository.GetQueryableAsync();
        var categoryQuery = await _categoryRepository.GetQueryableAsync();

        var completedOrders = orderQuery.Where(x => x.Status == OrderStatus.Completed);

        return new DashboardDto
        {
            TotalTrainers = await AsyncExecuter.CountAsync(trainerQuery),
            ActiveTrainers = await AsyncExecuter.CountAsync(trainerQuery.Where(x => x.IsActive)),
            VerifiedTrainers = await AsyncExecuter.CountAsync(trainerQuery.Where(x => x.IsVerified)),
            TotalOrders = await AsyncExecuter.CountAsync(orderQuery),
            PendingOrders = await AsyncExecuter.CountAsync(orderQuery.Where(x => x.Status == OrderStatus.Pending)),
            CompletedOrders = await AsyncExecuter.CountAsync(completedOrders),
            TotalRevenue = await AsyncExecuter.SumAsync(completedOrders.Select(x => x.NetAmount)),
            TotalCommission = await AsyncExecuter.SumAsync(completedOrders.Select(x => x.CommissionAmount)),
            TotalReviews = await AsyncExecuter.CountAsync(reviewQuery),
            AveragePlatformRating = await AsyncExecuter.CountAsync(reviewQuery) > 0
                ? (decimal)await AsyncExecuter.AverageAsync(reviewQuery.Select(x => (double)x.Rating))
                : 0m,
            TotalActiveSubscriptions = await AsyncExecuter.CountAsync(
                subscriptionQuery.Where(x => x.Status == SubscriptionStatus.Active)),
            TotalPackages = await AsyncExecuter.CountAsync(packageQuery),
            TotalCategories = await AsyncExecuter.CountAsync(categoryQuery)
        };
    }

    [Authorize(FitliyoPermissions.Admin.Dashboard)]
    public async Task<PlatformStatsDto> GetPlatformStatsAsync(DateTime startDate, DateTime endDate)
    {
        var orderQuery = await _orderRepository.GetQueryableAsync();
        var completedInRange = orderQuery.Where(x =>
            x.Status == OrderStatus.Completed &&
            x.CompletedAt >= startDate &&
            x.CompletedAt <= endDate);

        var dailyRevenue = await AsyncExecuter.ToListAsync(
            completedInRange
                .GroupBy(x => x.CompletedAt!.Value.Date)
                .Select(g => new DailyRevenueDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.NetAmount),
                    Commission = g.Sum(x => x.CommissionAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date));

        var trainerQuery = await _trainerProfileRepository.GetQueryableAsync();
        var topTrainers = await AsyncExecuter.ToListAsync(
            trainerQuery
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.TotalReviewCount)
                .Take(10)
                .Select(x => new TopTrainerDto
                {
                    TrainerProfileId = x.Id,
                    AverageRating = x.AverageRating,
                    OrderCount = x.TotalStudentCount
                }));

        return new PlatformStatsDto
        {
            DailyRevenue = dailyRevenue,
            TopTrainers = topTrainers
        };
    }

    [Authorize(FitliyoPermissions.Trainers.Verify)]
    public async Task VerifyTrainerAsync(Guid trainerProfileId)
    {
        var trainer = await _trainerProfileRepository.GetAsync(trainerProfileId);
        trainer.IsVerified = true;
        trainer.VerificationBadge = "verified";
        await _trainerProfileRepository.UpdateAsync(trainer);
        Logger.LogInformation("Eğitmen doğrulandı: {TrainerProfileId}", trainerProfileId);
    }

    [Authorize(FitliyoPermissions.Trainers.Verify)]
    public async Task UnverifyTrainerAsync(Guid trainerProfileId)
    {
        var trainer = await _trainerProfileRepository.GetAsync(trainerProfileId);
        trainer.IsVerified = false;
        trainer.VerificationBadge = null;
        await _trainerProfileRepository.UpdateAsync(trainer);
        Logger.LogInformation("Eğitmen doğrulaması kaldırıldı: {TrainerProfileId}", trainerProfileId);
    }

    [Authorize(FitliyoPermissions.Reviews.Delete)]
    public async Task ToggleReviewVisibilityAsync(Guid reviewId)
    {
        var review = await _reviewRepository.GetAsync(reviewId);
        review.IsHidden = !review.IsHidden;
        await _reviewRepository.UpdateAsync(review);
        Logger.LogInformation("Değerlendirme görünürlüğü değiştirildi: {ReviewId}, IsHidden: {IsHidden}", reviewId, review.IsHidden);
    }
}
