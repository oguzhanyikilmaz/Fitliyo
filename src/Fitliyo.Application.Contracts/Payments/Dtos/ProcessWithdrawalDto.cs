using System.ComponentModel.DataAnnotations;
using Fitliyo.Enums;

namespace Fitliyo.Payments.Dtos;

public class ProcessWithdrawalDto
{
    public WithdrawalRequestStatus Status { get; set; }

    [StringLength(500)]
    public string? AdminNote { get; set; }
}
