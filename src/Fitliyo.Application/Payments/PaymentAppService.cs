using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Payments.Dtos;
using Fitliyo.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Fitliyo.Payments;

[Authorize]
public class PaymentAppService : FitliyoAppService, IPaymentAppService
{
    private readonly IRepository<Payment, Guid> _paymentRepository;

    public PaymentAppService(IRepository<Payment, Guid> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    [Authorize(FitliyoPermissions.Payments.View)]
    public async Task<PaymentDto> GetAsync(Guid id)
    {
        var entity = await _paymentRepository.GetAsync(id);
        return ObjectMapper.Map<Payment, PaymentDto>(entity);
    }

    [Authorize(FitliyoPermissions.Payments.View)]
    public async Task<PagedResultDto<PaymentDto>> GetListAsync(GetPaymentListDto input)
    {
        var queryable = await _paymentRepository.GetQueryableAsync();

        if (input.OrderId.HasValue)
            queryable = queryable.Where(x => x.OrderId == input.OrderId.Value);
        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting)
            ? queryable.OrderBy(input.Sorting)
            : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);

        return new PagedResultDto<PaymentDto>(totalCount,
            items.Select(x => ObjectMapper.Map<Payment, PaymentDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Payments.Default)]
    public async Task<PaymentDto?> GetByOrderIdAsync(Guid orderId)
    {
        var entity = await _paymentRepository.FindAsync(x => x.OrderId == orderId);
        return entity == null ? null : ObjectMapper.Map<Payment, PaymentDto>(entity);
    }
}
