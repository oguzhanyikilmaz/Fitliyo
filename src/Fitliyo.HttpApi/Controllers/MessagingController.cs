using System;
using System.Threading.Tasks;
using Fitliyo.Messaging;
using Fitliyo.Messaging.Dtos;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Controllers;

[RemoteService]
[Area("app")]
[Route("api/app/messaging")]
public class MessagingController : FitliyoController, IMessagingAppService
{
    private readonly IMessagingAppService _messagingAppService;

    public MessagingController(IMessagingAppService messagingAppService)
    {
        _messagingAppService = messagingAppService;
    }

    [HttpGet("conversations")]
    public Task<ListResultDto<ConversationDto>> GetMyConversationsAsync()
    {
        return _messagingAppService.GetMyConversationsAsync();
    }

    [HttpGet("conversations/{conversationId}/messages")]
    public Task<PagedResultDto<MessageDto>> GetMessagesAsync(Guid conversationId, [FromQuery] PagedResultRequestDto input)
    {
        return _messagingAppService.GetMessagesAsync(conversationId, input);
    }

    [HttpPost("messages")]
    public Task<MessageDto> SendMessageAsync(SendMessageDto input)
    {
        return _messagingAppService.SendMessageAsync(input);
    }

    [HttpPost("conversations/{conversationId}/read")]
    public Task MarkAsReadAsync(Guid conversationId)
    {
        return _messagingAppService.MarkAsReadAsync(conversationId);
    }
}
