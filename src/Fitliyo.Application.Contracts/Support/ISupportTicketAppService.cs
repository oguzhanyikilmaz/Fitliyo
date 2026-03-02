using System;
using System.Threading.Tasks;
using Fitliyo.Enums;
using Fitliyo.Support.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Support;

public interface ISupportTicketAppService : IApplicationService
{
    Task<SupportTicketDto> CreateAsync(CreateSupportTicketDto input);
    Task<SupportTicketDto> GetAsync(Guid id);
    Task<PagedResultDto<SupportTicketDto>> GetMyTicketsAsync(GetSupportTicketListDto input);
    Task<PagedResultDto<SupportTicketDto>> GetListAsync(GetSupportTicketListDto input);
    Task<SupportTicketDto> ReplyAsync(Guid id, ReplySupportTicketDto input);
    Task<SupportTicketDto> UpdateStatusAsync(Guid id, SupportTicketStatus status);
}
