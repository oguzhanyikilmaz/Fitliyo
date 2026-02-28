using System;
using System.Threading.Tasks;
using Fitliyo.Reviews;
using Fitliyo.Reviews.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/reviews")]
public class ReviewController : FitliyoController, IReviewAppService
{
    private readonly IReviewAppService _reviewAppService;

    public ReviewController(IReviewAppService reviewAppService)
    {
        _reviewAppService = reviewAppService;
    }

    [HttpGet("{id}")]
    public Task<ReviewDto> GetAsync(Guid id)
    {
        return _reviewAppService.GetAsync(id);
    }

    [HttpGet]
    public Task<PagedResultDto<ReviewDto>> GetListByTrainerAsync([FromQuery] GetReviewListDto input)
    {
        return _reviewAppService.GetListByTrainerAsync(input);
    }

    [HttpPost]
    public Task<ReviewDto> CreateAsync(CreateReviewDto input)
    {
        return _reviewAppService.CreateAsync(input);
    }

    [HttpPost("{id}/reply")]
    public Task<ReviewDto> ReplyAsync(Guid id, ReplyToReviewDto input)
    {
        return _reviewAppService.ReplyAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _reviewAppService.DeleteAsync(id);
    }
}
