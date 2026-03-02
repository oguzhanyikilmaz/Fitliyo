using System;
using System.Threading.Tasks;
using Fitliyo.Payments.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Payments;

public interface IWithdrawalRequestAppService : IApplicationService
{
    Task<WithdrawalRequestDto> CreateAsync(CreateWithdrawalRequestDto input);
    Task<WithdrawalRequestDto> GetAsync(Guid id);
    Task<PagedResultDto<WithdrawalRequestDto>> GetMyRequestsAsync(GetWithdrawalRequestListDto input);
    Task<PagedResultDto<WithdrawalRequestDto>> GetListAsync(GetWithdrawalRequestListDto input);
    Task<WithdrawalRequestDto> ApproveAsync(Guid id, ProcessWithdrawalDto? input = null);
    Task<WithdrawalRequestDto> RejectAsync(Guid id, ProcessWithdrawalDto? input = null);
    Task<WithdrawalRequestDto> MarkProcessedAsync(Guid id);
}
