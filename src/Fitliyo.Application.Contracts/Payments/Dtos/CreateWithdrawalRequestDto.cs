using System.ComponentModel.DataAnnotations;

namespace Fitliyo.Payments.Dtos;

public class CreateWithdrawalRequestDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(34)]
    public string Iban { get; set; } = default!;

    [Required]
    [StringLength(256)]
    public string AccountHolderName { get; set; } = default!;
}
