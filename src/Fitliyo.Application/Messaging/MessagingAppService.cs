using System;
using System.Linq;
using System.Threading.Tasks;
using Fitliyo.Messaging.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Fitliyo.Messaging;

[Authorize]
public class MessagingAppService : FitliyoAppService, IMessagingAppService
{
    private readonly IRepository<Conversation, Guid> _conversationRepository;
    private readonly IRepository<Message, Guid> _messageRepository;

    public MessagingAppService(
        IRepository<Conversation, Guid> conversationRepository,
        IRepository<Message, Guid> messageRepository)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
    }

    [Authorize]
    public async Task<ListResultDto<ConversationDto>> GetMyConversationsAsync()
    {
        var userId = CurrentUser.GetId();
        var conversations = await _conversationRepository.GetListAsync(
            x => (x.InitiatorId == userId || x.ParticipantId == userId) && x.IsActive);

        var sorted = conversations.OrderByDescending(x => x.LastMessageAt).ToList();
        var dtos = sorted.Select(x => ObjectMapper.Map<Conversation, ConversationDto>(x)).ToList();

        return new ListResultDto<ConversationDto>(dtos);
    }

    [Authorize]
    public async Task<PagedResultDto<MessageDto>> GetMessagesAsync(Guid conversationId, PagedResultRequestDto input)
    {
        var userId = CurrentUser.GetId();
        var conversation = await _conversationRepository.GetAsync(conversationId);

        if (conversation.InitiatorId != userId && conversation.ParticipantId != userId)
            throw new BusinessException(FitliyoDomainErrorCodes.ConversationNotFound);

        var queryable = await _messageRepository.GetQueryableAsync();
        queryable = queryable.Where(x => x.ConversationId == conversationId);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        queryable = queryable.OrderByDescending(x => x.CreationTime);
        queryable = queryable.PageBy(input);

        var entities = await AsyncExecuter.ToListAsync(queryable);

        var dtos = entities.Select(m =>
        {
            var dto = ObjectMapper.Map<Message, MessageDto>(m);
            dto.IsMine = m.SenderId == userId;
            return dto;
        }).ToList();

        return new PagedResultDto<MessageDto>(totalCount, dtos);
    }

    [Authorize]
    public async Task<MessageDto> SendMessageAsync(SendMessageDto input)
    {
        var userId = CurrentUser.GetId();

        if (input.RecipientId == userId)
            throw new BusinessException(FitliyoDomainErrorCodes.CannotMessageSelf);

        var conversation = await GetOrCreateConversationAsync(userId, input.RecipientId);

        var message = new Message(GuidGenerator.Create(), conversation.Id, userId, input.Content);
        message.AttachmentUrl = input.AttachmentUrl;

        await _messageRepository.InsertAsync(message);

        conversation.LastMessageAt = DateTime.Now;
        await _conversationRepository.UpdateAsync(conversation);

        Logger.LogInformation("Mesaj gönderildi: {ConversationId}, Gönderen: {SenderId}", conversation.Id, userId);

        var dto = ObjectMapper.Map<Message, MessageDto>(message);
        dto.IsMine = true;
        return dto;
    }

    [Authorize]
    public async Task MarkAsReadAsync(Guid conversationId)
    {
        var userId = CurrentUser.GetId();
        var unreadMessages = await _messageRepository.GetListAsync(
            x => x.ConversationId == conversationId && x.SenderId != userId && !x.IsRead);

        foreach (var message in unreadMessages)
        {
            message.MarkAsRead();
        }

        if (unreadMessages.Count > 0)
        {
            await _messageRepository.UpdateManyAsync(unreadMessages);
        }
    }

    private async Task<Conversation> GetOrCreateConversationAsync(Guid userId, Guid recipientId)
    {
        var existing = await _conversationRepository.FindAsync(x =>
            (x.InitiatorId == userId && x.ParticipantId == recipientId) ||
            (x.InitiatorId == recipientId && x.ParticipantId == userId));

        if (existing != null)
        {
            if (!existing.IsActive) existing.IsActive = true;
            return existing;
        }

        var conversation = new Conversation(GuidGenerator.Create(), userId, recipientId);
        await _conversationRepository.InsertAsync(conversation);
        return conversation;
    }
}
