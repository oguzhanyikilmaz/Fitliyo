using System.Threading.Tasks;
using Fitliyo.Payments.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Payments;

public interface IWalletAppService : IApplicationService
{
    Task<TrainerWalletDto> GetMyWalletAsync();
    Task<PagedResultDto<WalletTransactionDto>> GetMyTransactionsAsync(GetWalletTransactionListDto input);
}
