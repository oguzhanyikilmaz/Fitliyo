using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Payments.Dtos;
using Fitliyo.Permissions;
using Fitliyo.Trainers;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Payments;

[Authorize]
public class WithdrawalRequestAppService : FitliyoAppService, IWithdrawalRequestAppService
{
    private readonly IRepository<WithdrawalRequest, Guid> _withdrawalRepository;
    private readonly IRepository<TrainerWallet, Guid> _walletRepository;
    private readonly IRepository<TrainerProfile, Guid> _trainerProfileRepository;

    public WithdrawalRequestAppService(
        IRepository<WithdrawalRequest, Guid> withdrawalRepository,
        IRepository<TrainerWallet, Guid> walletRepository,
        IRepository<TrainerProfile, Guid> trainerProfileRepository)
    {
        _withdrawalRepository = withdrawalRepository;
        _walletRepository = walletRepository;
        _trainerProfileRepository = trainerProfileRepository;
    }

    [Authorize(FitliyoPermissions.Withdrawal.Default)]
    public async Task<WithdrawalRequestDto> CreateAsync(CreateWithdrawalRequestDto input)
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var trainer = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (trainer == null)
            throw new Volo.Abp.BusinessException(FitliyoDomainErrorCodes.TrainerProfileNotFound);

        var wallet = await _walletRepository.FindAsync(x => x.TrainerProfileId == trainer!.Id);
        if (wallet == null)
            throw new Volo.Abp.BusinessException(FitliyoDomainErrorCodes.WalletNotFound);
        if (wallet.AvailableBalance < input.Amount)
            throw new Volo.Abp.BusinessException(FitliyoDomainErrorCodes.InsufficientBalance);

        var entity = new WithdrawalRequest(
            GuidGenerator.Create(),
            wallet.Id,
            input.Amount,
            input.Iban,
            input.AccountHolderName);
        await _withdrawalRepository.InsertAsync(entity);
        return ObjectMapper.Map<WithdrawalRequest, WithdrawalRequestDto>(entity);
    }

    [Authorize(FitliyoPermissions.Withdrawal.Default)]
    public async Task<WithdrawalRequestDto> GetAsync(Guid id)
    {
        var entity = await _withdrawalRepository.GetAsync(id);
        return ObjectMapper.Map<WithdrawalRequest, WithdrawalRequestDto>(entity);
    }

    [Authorize(FitliyoPermissions.Withdrawal.Default)]
    public async Task<PagedResultDto<WithdrawalRequestDto>> GetMyRequestsAsync(GetWithdrawalRequestListDto input)
    {
        var wallet = await GetMyWalletOrNullAsync();
        if (wallet == null)
            return new PagedResultDto<WithdrawalRequestDto>(0, new System.Collections.Generic.List<WithdrawalRequestDto>());

        var queryable = await _withdrawalRepository.GetQueryableAsync();
        queryable = queryable.Where(x => x.TrainerWalletId == wallet.Id);
        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<WithdrawalRequestDto>(totalCount, items.Select(x => ObjectMapper.Map<WithdrawalRequest, WithdrawalRequestDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Withdrawal.Manage)]
    public async Task<PagedResultDto<WithdrawalRequestDto>> GetListAsync(GetWithdrawalRequestListDto input)
    {
        var queryable = await _withdrawalRepository.GetQueryableAsync();
        if (input.Status.HasValue)
            queryable = queryable.Where(x => x.Status == input.Status.Value);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        queryable = !string.IsNullOrWhiteSpace(input.Sorting) ? queryable.OrderBy(input.Sorting) : queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);
        var items = await AsyncExecuter.ToListAsync(queryable);
        return new PagedResultDto<WithdrawalRequestDto>(totalCount, items.Select(x => ObjectMapper.Map<WithdrawalRequest, WithdrawalRequestDto>(x)).ToList());
    }

    [Authorize(FitliyoPermissions.Withdrawal.Manage)]
    public async Task<WithdrawalRequestDto> ApproveAsync(Guid id, ProcessWithdrawalDto? input = null)
    {
        var entity = await _withdrawalRepository.GetAsync(id);
        entity.Approve(input?.AdminNote);
        await _withdrawalRepository.UpdateAsync(entity);
        return ObjectMapper.Map<WithdrawalRequest, WithdrawalRequestDto>(entity);
    }

    [Authorize(FitliyoPermissions.Withdrawal.Manage)]
    public async Task<WithdrawalRequestDto> RejectAsync(Guid id, ProcessWithdrawalDto? input = null)
    {
        var entity = await _withdrawalRepository.GetAsync(id);
        entity.Reject(input?.AdminNote);
        await _withdrawalRepository.UpdateAsync(entity);
        return ObjectMapper.Map<WithdrawalRequest, WithdrawalRequestDto>(entity);
    }

    [Authorize(FitliyoPermissions.Withdrawal.Manage)]
    public async Task<WithdrawalRequestDto> MarkProcessedAsync(Guid id)
    {
        var entity = await _withdrawalRepository.GetAsync(id);
        if (entity.Status != WithdrawalRequestStatus.Approved)
            throw new Volo.Abp.BusinessException(FitliyoDomainErrorCodes.WithdrawalRequestNotFound, "Sadece onaylanmış talepler işlendi olarak işaretlenebilir.");
        var wallet = await _walletRepository.GetAsync(entity.TrainerWalletId);
        wallet.DebitAvailable(entity.Amount);
        await _walletRepository.UpdateAsync(wallet);
        entity.MarkProcessed();
        await _withdrawalRepository.UpdateAsync(entity);
        return ObjectMapper.Map<WithdrawalRequest, WithdrawalRequestDto>(entity);
    }

    private async Task<TrainerWallet?> GetMyWalletOrNullAsync()
    {
        var userId = (CurrentUser.Id ?? Guid.Empty);
        var trainer = await _trainerProfileRepository.FindAsync(x => x.UserId == userId);
        if (trainer == null) return null;
        return await _walletRepository.FindAsync(x => x.TrainerProfileId == trainer.Id);
    }
}
