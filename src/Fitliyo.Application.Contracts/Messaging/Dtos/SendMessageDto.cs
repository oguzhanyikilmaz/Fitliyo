using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Messaging;

namespace Fitliyo.Messaging.Dtos;

public class SendMessageDto
{
    [Required]
    public Guid RecipientId { get; set; }

    [Required]
    [StringLength(MessageConsts.MaxContentLength)]
    public string Content { get; set; } = default!;

    [StringLength(MessageConsts.MaxAttachmentUrlLength)]
    public string? AttachmentUrl { get; set; }
}
