using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Orders;
using Fitliyo.Payments;
using Fitliyo.Reviews;
using Fitliyo.ServicePackages;
using Fitliyo.Subscriptions;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Fitliyo.Web.Pages;

[Authorize]
public class IndexModel : FitliyoPageModel
{
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<ServicePackage, Guid> _servicePackageRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<SubscriptionPlan, Guid> _subscriptionPlanRepository;
    private readonly IRepository<TrainerSubscription, Guid> _trainerSubscriptionRepository;
    private readonly IRepository<Review, Guid> _reviewRepository;
    private readonly IRepository<WithdrawalRequest, Guid> _withdrawalRequestRepository;

    public DashboardStats Stats { get; private set; } = new();
    public List<string> MonthlyLabels { get; private set; } = [];
    public List<int> MonthlyOrderCounts { get; private set; } = [];
    public List<decimal> MonthlyRevenue { get; private set; } = [];
    public List<string> PackageTypeLabels { get; private set; } = [];
    public List<int> PackageTypeCounts { get; private set; } = [];

    public IndexModel(
        IRepository<IdentityUser, Guid> identityUserRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<ServicePackage, Guid> servicePackageRepository,
        IRepository<Order, Guid> orderRepository,
        IRepository<SubscriptionPlan, Guid> subscriptionPlanRepository,
        IRepository<TrainerSubscription, Guid> trainerSubscriptionRepository,
        IRepository<Review, Guid> reviewRepository,
        IRepository<WithdrawalRequest, Guid> withdrawalRequestRepository)
    {
        _identityUserRepository = identityUserRepository;
        _trainerProfileRepository = trainerProfileRepository;
        _servicePackageRepository = servicePackageRepository;
        _orderRepository = orderRepository;
        _subscriptionPlanRepository = subscriptionPlanRepository;
        _trainerSubscriptionRepository = trainerSubscriptionRepository;
        _reviewRepository = reviewRepository;
        _withdrawalRequestRepository = withdrawalRequestRepository;
    }

    public async Task OnGetAsync()
    {
        Stats = await BuildStatsAsync();
        await BuildMonthlyOrderChartAsync();
        await BuildPackageTypeChartAsync();
    }

    private async Task<DashboardStats> BuildStatsAsync()
    {
        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        var totalUsers = await _identityUserRepository.GetCountAsync();
        var totalOrders = await _orderRepository.GetCountAsync();
        var totalSubscriptions = await _trainerSubscriptionRepository.GetCountAsync();
        var totalSubscriptionPlans = await _subscriptionPlanRepository.GetCountAsync();
        var trainerQuery = await _trainerProfileRepository.GetQueryableAsync();
        var packageQuery = await _servicePackageRepository.GetQueryableAsync();
        var reviewQuery = await _reviewRepository.GetQueryableAsync();
        var withdrawalQuery = await _withdrawalRequestRepository.GetQueryableAsync();
        var ordersQuery = await _orderRepository.GetQueryableAsync();

        var totalTrainers = trainerQuery.LongCount(x => x.IsActive);
        var totalPackages = packageQuery.LongCount(x => x.IsActive);
        var totalReviews = reviewQuery.LongCount(x => !x.IsHidden);
        var pendingWithdrawals = withdrawalQuery.LongCount(x => x.Status == WithdrawalRequestStatus.Pending);
        var monthlyOrderCount = ordersQuery.LongCount(x => x.CreationTime >= monthStart);

        var monthlyRevenue = ordersQuery
            .Where(x => x.CreationTime >= monthStart && x.PaymentStatus != PaymentStatus.Refunded)
            .Select(x => (decimal?)x.NetAmount)
            .Sum() ?? 0m;

        return new DashboardStats
        {
            TotalUsers = totalUsers,
            TotalTrainers = totalTrainers,
            TotalPackages = totalPackages,
            TotalOrders = totalOrders,
            TotalSubscriptions = totalSubscriptions,
            TotalSubscriptionPlans = totalSubscriptionPlans,
            TotalReviews = totalReviews,
            PendingWithdrawals = pendingWithdrawals,
            MonthlyOrderCount = monthlyOrderCount,
            MonthlyRevenue = monthlyRevenue
        };
    }

    private async Task BuildMonthlyOrderChartAsync()
    {
        var now = DateTime.Now;
        var start = new DateTime(now.Year, now.Month, 1).AddMonths(-5);

        var ordersQuery = await _orderRepository.GetQueryableAsync();
        var recentOrders = ordersQuery
            .Where(x => x.CreationTime >= start)
            .Select(x => new { x.CreationTime, x.NetAmount, x.PaymentStatus })
            .ToList();

        for (var i = 0; i < 6; i++)
        {
            var monthStart = start.AddMonths(i);
            var monthEnd = monthStart.AddMonths(1);
            MonthlyLabels.Add(monthStart.ToString("MMM yyyy"));

            var monthOrders = recentOrders.Where(x => x.CreationTime >= monthStart && x.CreationTime < monthEnd).ToList();
            MonthlyOrderCounts.Add(monthOrders.Count);
            MonthlyRevenue.Add(monthOrders
                .Where(x => x.PaymentStatus != PaymentStatus.Refunded)
                .Select(x => x.NetAmount)
                .Sum());
        }
    }

    private async Task BuildPackageTypeChartAsync()
    {
        var packageQuery = await _servicePackageRepository.GetQueryableAsync();
        var packages = packageQuery
            .Where(x => x.IsActive)
            .Select(x => x.PackageType)
            .ToList();

        PackageTypeLabels = Enum.GetValues<PackageType>()
            .Select(GetPackageTypeLabel)
            .ToList();

        PackageTypeCounts = Enum.GetValues<PackageType>()
            .Select(type => packages.Count(x => x == type))
            .ToList();
    }

    private static string GetPackageTypeLabel(PackageType type)
    {
        return type switch
        {
            PackageType.SingleSession => "Tek Seans",
            PackageType.Training => "Antrenman",
            PackageType.Nutrition => "Beslenme",
            PackageType.Combined => "Kombine",
            PackageType.GroupSession => "Grup Seansı",
            _ => type.ToString()
        };
    }

    public class DashboardStats
    {
        public long TotalUsers { get; set; }
        public long TotalTrainers { get; set; }
        public long TotalPackages { get; set; }
        public long TotalOrders { get; set; }
        public long TotalSubscriptions { get; set; }
        public long TotalSubscriptionPlans { get; set; }
        public long TotalReviews { get; set; }
        public long PendingWithdrawals { get; set; }
        public long MonthlyOrderCount { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }
}
