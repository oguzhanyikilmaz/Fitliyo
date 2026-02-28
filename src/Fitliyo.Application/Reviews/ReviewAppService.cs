using System;
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
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Reviews;

[Authorize]
public class ReviewAppService : FitliyoAppService, IReviewAppService
{
    private readonly IRepository<Review, Guid> _reviewRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;
    private readonly FitliyoApplicationMappers _mapper;

    public ReviewAppService(
        IRepository<Review, Guid> reviewRepository,
        IRepository<Order, Guid> orderRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository,
        FitliyoApplicationMappers mapper)
    {
        _reviewRepository = reviewRepository;
        _orderRepository = orderRepository;
        _trainerProfileRepository = trainerProfileRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<ReviewDto> GetAsync(Guid id)
    {
        var review = await _reviewRepository.GetAsync(id);
        return _mapper.ReviewToDto(review);
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

        return new PagedResultDto<ReviewDto>(totalCount, entities.Select(_mapper.ReviewToDto).ToList());
    }

    [Authorize]
    public async Task<ReviewDto> CreateAsync(CreateReviewDto input)
    {
        var userId = CurrentUser.GetId();
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

        await _reviewRepository.InsertAsync(review);

        await UpdateTrainerRatingAsync(order.TrainerProfileId);

        Logger.LogInformation("Değerlendirme oluşturuldu: {ReviewId}, Sipariş: {OrderId}, Puan: {Rating}", review.Id, order.Id, input.Rating);

        return _mapper.ReviewToDto(review);
    }

    [Authorize]
    public async Task<ReviewDto> ReplyAsync(Guid id, ReplyToReviewDto input)
    {
        var review = await _reviewRepository.GetAsync(id);
        var userId = CurrentUser.GetId();

        var trainerProfile = await _trainerProfileRepository.GetAsync(review.TrainerProfileId);
        if (trainerProfile.UserId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.UnauthorizedAccess);

        review.SetTrainerReply(input.Reply);
        await _reviewRepository.UpdateAsync(review);

        Logger.LogInformation("Değerlendirme yanıtlandı: {ReviewId}", id);

        return _mapper.ReviewToDto(review);
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
}
