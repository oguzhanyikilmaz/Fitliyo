using System;
using System.Threading.Tasks;
using Fitliyo.Messaging.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Fitliyo.Messaging;

public interface IMessagingAppService : IApplicationService
{
    Task<ListResultDto<ConversationDto>> GetMyConversationsAsync();

    Task<PagedResultDto<MessageDto>> GetMessagesAsync(Guid conversationId, PagedResultRequestDto input);

    Task<MessageDto> SendMessageAsync(SendMessageDto input);

    Task MarkAsReadAsync(Guid conversationId);
}
