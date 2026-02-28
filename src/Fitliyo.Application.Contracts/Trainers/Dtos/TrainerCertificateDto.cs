using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Trainers.Dtos;

public class TrainerCertificateDto : EntityDto<Guid>
{
    public Guid TrainerProfileId { get; set; }
    public string CertificateName { get; set; } = default!;
    public string? IssuingOrganization { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? DocumentUrl { get; set; }
    public bool IsVerifiedByPlatform { get; set; }
}
