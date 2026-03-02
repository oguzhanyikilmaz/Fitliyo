using System.ComponentModel.DataAnnotations;
using Fitliyo.Orders;

namespace Fitliyo.Orders.Dtos;

/// <summary>
/// Eğitmenin siparişe özel program teslimi (notlar ve dosya/link).
/// </summary>
public class UpdateOrderDeliveryDto
{
    [StringLength(OrderConsts.MaxTrainerProgramNotesLength)]
    public string? TrainerProgramNotes { get; set; }

    [StringLength(OrderConsts.MaxProgramAttachmentUrlLength)]
    public string? ProgramAttachmentUrl { get; set; }

    /// <summary>
    /// true ise ProgramDeliveredAt şimdi set edilir (program teslim edildi olarak işaretlenir).
    /// </summary>
    public bool MarkAsDelivered { get; set; }
}
