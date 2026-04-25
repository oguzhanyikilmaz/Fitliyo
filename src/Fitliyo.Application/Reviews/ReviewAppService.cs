using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Orders;
using Fitliyo.Permissions;
using Fitliyo.Reviews.Dtos;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Fitliyo.Reviews;

[Authorize]
public class ReviewAppService : FitliyoAppService, IReviewAppService
{
    private readonly IRepository<Review, Guid> _reviewRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly IRepository<IdentityUser, Guid> _identityUserRepository;

    public ReviewAppService(
        IRepository<Review, Guid> reviewRepository,
        IRepository<Order, Guid> orderRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        IRepository<IdentityUser, Guid> identityUserRepository)
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _trainerProfileRepository = trainerProfileRepository;
        _identityUserRepository = identityUserRepository;
    }

    [AllowAnonymous]
    public async Task<ReviewDto> GetAsync(Guid id)
    {
        var review = await _reviewRepository.GetAsync(id);
        await EnrichReviewDisplayNamesAsync([review]);
        return MapReviewToDto(review);
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<ReviewDto>> GetListByTrainerAsync(GetReviewListDto input)
    {
        var queryable = await _reviewRepository.GetQueryableAsync();

        queryable = queryable.Where(x => !x.IsHidden);

        if (input.TrainerProfileId.HasValue)
            queryable = queryable.Where(x => x.TrainerProfileId == input.TrainerProfileId.Value);

        if (input.MinRating.HasValue)
            queryable = queryable.Where(x => x.Rating >= input.MinRating.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = !string.IsNullOrWhiteSpace(input.Sorting)
            ? queryable.OrderBy(input.Sorting)
            : queryable.OrderByDescending(x => x.CreationTime);

        queryable = queryable.PageBy(input);
        var entities = await AsyncExecuter.ToListAsync(queryable);
        await EnrichReviewDisplayNamesAsync(entities);
        var dtos = entities.Select(MapReviewToDto).ToList();
        return new PagedResultDto<ReviewDto>(totalCount, dtos);
    }

    [Authorize]
    public async Task<ReviewDto> CreateAsync(CreateReviewDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var order = await _orderRepository.GetAsync(input.OrderId);

        if (order.StudentId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);

        if (order.Status != OrderStatus.Completed)
            throw new BusinessException(FitliyoDomainErrorCodes.OrderNotCompletedForReview);

        var existingReview = await _reviewRepository.FindAsync(x => x.OrderId == input.OrderId);
        if (existingReview != null)
            throw new BusinessException(FitliyoDomainErrorCodes.ReviewAlreadyExists);

        if (order.CompletedAt.HasValue)
        {
            var daysSinceCompletion = (DateTime.Now - order.CompletedAt.Value).TotalDays;
            if (daysSinceCompletion > ReviewConsts.MaxReviewDaysAfterCompletion)
                throw new BusinessException(FitliyoDomainErrorCodes.ReviewPeriodExpired);
        }

        var review = new Review(GuidGenerator.Create(), order.Id, userId, order.TrainerProfileId, input.Rating);
        review.Comment = input.Comment;
        await PopulateReviewDisplayNamesAsync(review, order.TrainerProfileId);

        await _reviewRepository.InsertAsync(review);

        await UpdateTrainerRatingAsync(order.TrainerProfileId);

        Logger.LogInformation("Değerlendirme oluşturuldu: {ReviewId}, Sipariş: {OrderId}, Puan: {Rating}", review.Id, order.Id, input.Rating);

        return MapReviewToDto(review);
    }

    [Authorize]
    public async Task<ReviewDto> ReplyAsync(Guid id, ReplyToReviewDto input)
    {
        var review = await _reviewRepository.GetAsync(id);
        var userId = (CurrentUser.Id ?? Guid.Empty);

        var trainerProfile = await _trainerProfileRepository.GetAsync(review.TrainerProfileId);
        if (trainerProfile.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);

        review.SetTrainerReply(input.Reply);
        await _reviewRepository.UpdateAsync(review);

        Logger.LogInformation("Değerlendirme yanıtlandı: {ReviewId}", id);

        await EnrichReviewDisplayNamesAsync([review]);
        return MapReviewToDto(review);
    }

    [Authorize(FitliyoPermissions.Admin.Dashboard)]
    public async Task DeleteAsync(Guid id)
    {
        await _reviewRepository.DeleteAsync(id);
        Logger.LogInformation("Değerlendirme silindi: {ReviewId}", id);
    }

    private async Task UpdateTrainerRatingAsync(Guid trainerProfileId)
    {
        var reviews = await _reviewRepository.GetListAsync(x => x.TrainerProfileId == trainerProfileId && !x.IsHidden);
        if (reviews.Count == 0) return;

        var trainerProfile = await _trainerProfileRepository.GetAsync(trainerProfileId);
        trainerProfile.AverageRating = (decimal)reviews.Average(x => x.Rating);
        trainerProfile.TotalReviewCount = reviews.Count;
        await _trainerProfileRepository.UpdateAsync(trainerProfile);
    }

    private async Task PopulateReviewDisplayNamesAsync(Review review, Guid trainerProfileId)
    {
        var trainerProfile = await _trainerProfileRepository.GetAsync(trainerProfileId);
        var student = await _identityUserRepository.FindAsync(review.StudentId);
        var trainerUser = await _identityUserRepository.FindAsync(trainerProfile.UserId);

        var studentFullName = BuildFullName(student?.Name, student?.Surname);
        var trainerFullName = BuildFullName(trainerUser?.Name, trainerUser?.Surname);

        if (!string.IsNullOrWhiteSpace(studentFullName))
        {
            review.SetProperty("StudentFullName", studentFullName);
        }

        if (!string.IsNullOrWhiteSpace(trainerFullName))
        {
            review.SetProperty("TrainerFullName", trainerFullName);
        }
    }

    private async Task EnrichReviewDisplayNamesAsync(IReadOnlyList<Review> reviews)
    {
        if (reviews.Count == 0)
        {
            return;
        }

        var studentIds = reviews.Select(x => x.StudentId).Distinct().ToList();
        var trainerProfileIds = reviews.Select(x => x.TrainerProfileId).Distinct().ToList();

        var trainerProfilesQuery = await _trainerProfileRepository.GetQueryableAsync();
        var trainerProfiles = await AsyncExecuter.ToListAsync(trainerProfilesQuery.Where(x => trainerProfileIds.Contains(x.Id)));
        var trainerProfileMap = trainerProfiles.ToDictionary(x => x.Id, x => x.UserId);

        var userIds = studentIds
            .Concat(trainerProfiles.Select(x => x.UserId))
            .Distinct()
            .ToList();
        var usersQuery = await _identityUserRepository.GetQueryableAsync();
        var users = await AsyncExecuter.ToListAsync(usersQuery.Where(x => userIds.Contains(x.Id)));
        var userNameMap = users.ToDictionary(x => x.Id, x => BuildFullName(x.Name, x.Surname));

        var dirtyReviews = new List<Review>();
        foreach (var review in reviews)
        {
            var studentFullName = review.GetProperty<string>("StudentFullName");
            if (string.IsNullOrWhiteSpace(studentFullName) && userNameMap.TryGetValue(review.StudentId, out var resolvedStudentName) && !string.IsNullOrWhiteSpace(resolvedStudentName))
            {
                review.SetProperty("StudentFullName", resolvedStudentName);
                dirtyReviews.Add(review);
            }

            var trainerFullName = review.GetProperty<string>("TrainerFullName");
            if (string.IsNullOrWhiteSpace(trainerFullName)
                && trainerProfileMap.TryGetValue(review.TrainerProfileId, out var trainerUserId)
                && userNameMap.TryGetValue(trainerUserId, out var resolvedTrainerName)
                && !string.IsNullOrWhiteSpace(resolvedTrainerName))
            {
                review.SetProperty("TrainerFullName", resolvedTrainerName);
                if (!dirtyReviews.Contains(review))
                {
                    dirtyReviews.Add(review);
                }
            }
        }

        if (dirtyReviews.Count > 0)
        {
            await _reviewRepository.UpdateManyAsync(dirtyReviews);
        }
    }

    private ReviewDto MapReviewToDto(Review review)
    {
        var dto = ObjectMapper.Map<Review, ReviewDto>(review);
        dto.StudentFullName = review.GetProperty<string>("StudentFullName");
        dto.TrainerFullName = review.GetProperty<string>("TrainerFullName");
        return dto;
    }

    private static string? BuildFullName(string? name, string? surname)
    {
        var fullName = $"{name} {surname}".Trim();
        return string.IsNullOrWhiteSpace(fullName) ? null : fullName;
    }
}
