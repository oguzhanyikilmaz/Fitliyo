using System;
using System.Threading.Tasks;
using Fitliyo.Payments.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Payments;

public interface IPaymentAppService : IApplicationService
{
    Task<PaymentDto> GetAsync(Guid id);
    Task<PagedResultDto<PaymentDto>> GetListAsync(GetPaymentListDto input);
    Task<PaymentDto?> GetByOrderIdAsync(Guid orderId);
}
