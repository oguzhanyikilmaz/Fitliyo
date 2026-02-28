using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Trainers;

namespace Fitliyo.Trainers.Dtos;

public class CreateUpdateTrainerCertificateDto
{
    [Required]
    [StringLength(TrainerConsts.MaxCertificateNameLength)]
    public string CertificateName { get; set; } = default!;

    [StringLength(TrainerConsts.MaxIssuingOrganizationLength)]
    public string? IssuingOrganization { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [StringLength(TrainerConsts.MaxDocumentUrlLength)]
    public string? DocumentUrl { get; set; }
}
