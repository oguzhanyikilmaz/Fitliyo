using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Payments.Dtos;
using Fitliyo.Permissions;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Payments;

[Authorize(FitliyoPermissions.Wallet.Default)]
public class WalletAppService : FitliyoAppService, IWalletAppService
{
    private readonly IRepository<TrainerWallet, Guid> _walletRepository;
    private readonly IRepository<WalletTransaction, Guid> _transactionRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;

    public WalletAppService(
        IRepository<TrainerWallet, Guid> walletRepository,
        IRepository<WalletTransaction, Guid> transactionRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _trainerProfileRepository = trainerProfileRepository;
    }

    [Authorize]
    public async Task<TrainerWalletDto> GetMyWalletAsync()
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var trainer = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (trainer == null)
            throw new Volo.Abp.BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);

        var wallet = await _walletRepository.FindAsync(x => x.TrainerProfileId == trainer!.Id);
        if (wallet == null)
        {
            wallet = new TrainerWallet(GuidGenerator.Create(), trainer!.Id);
            await _walletRepository.InsertAsync(wallet);
        }

        return ObjectMapper.Map<TrainerWallet, TrainerWalletDto>(wallet);
    }

    [Authorize]
    public async Task<PagedResultDto<WalletTransactionDto>> GetMyTransactionsAsync(GetWalletTransactionListDto input)
    {
        var wallet = await GetMyWalletAsync();
        var queryable = await _transactionRepository.GetQueryableAsync();
        queryable = queryable.Where(x => x.TrainerWalletId == wallet.Id);

        if (input.TransactionType.HasValue)
            queryable = queryable.Where(x => x.TransactionType == input.TransactionType.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting)
            ? queryable.OrderBy(input.Sorting)
            : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);

        return new PagedResultDto<WalletTransactionDto>(totalCount,
            items.Select(x => ObjectMapper.Map<WalletTransaction, WalletTransactionDto>(x)).ToList());
    }
}
