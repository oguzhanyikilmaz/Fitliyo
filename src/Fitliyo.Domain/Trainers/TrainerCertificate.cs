using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Trainers;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Trainers;

/// <summary>
/// Eğitmen sertifikası
/// </summary>
public class TrainerCertificate : FullAuditedEntity<Guid>
{
    [Required]
    public Guid TrainerProfileId { get; private set; }

    [Required]
    [StringLength(TrainerConsts.MaxCertificateNameLength)]
    public string CertificateName { get; set; } = default!;

    [StringLength(TrainerConsts.MaxIssuingOrganizationLength)]
    public string? IssuingOrganization { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [StringLength(TrainerConsts.MaxDocumentUrlLength)]
    public string? DocumentUrl { get; set; }

    /// <summary>
    /// Platform tarafından doğrulandı mı
    /// </summary>
    public bool IsVerifiedByPlatform { get; set; }

    protected TrainerCertificate()
    {
    }

    public TrainerCertificate(Guid id, Guid trainerProfileId, string certificateName)
        : base(id)
    {
        TrainerProfileId = trainerProfileId;
        CertificateName = Check.NotNullOrWhiteSpace(certificateName, nameof(certificateName), TrainerConsts.MaxCertificateNameLength);
    }
}
