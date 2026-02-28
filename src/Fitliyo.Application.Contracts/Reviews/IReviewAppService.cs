using System;
using System.Threading.Tasks;
using Fitliyo.Reviews.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Reviews;

public interface IReviewAppService : IApplicationService
{
    Task<ReviewDto> GetAsync(Guid id);

    Task<PagedResultDto<ReviewDto>> GetListByTrainerAsync(GetReviewListDto input);

    Task<ReviewDto> CreateAsync(CreateReviewDto input);

    Task<ReviewDto> ReplyAsync(Guid id, ReplyToReviewDto input);

    Task DeleteAsync(Guid id);
}
