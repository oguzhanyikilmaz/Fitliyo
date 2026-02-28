using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Categories;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Fitliyo.Categories;

/// <summary>
/// Hizmet kategorisi (hiyerarşik — spor dalları, uzmanlık alanları)
/// </summary>
public class Category : FullAuditedAggregateRoot<Guid>
{
    [Required]
    [StringLength(CategoryConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(CategoryConsts.MaxSlugLength)]
    public string Slug { get; private set; } = default!;

    /// <summary>
    /// Üst kategori (null ise kök kategori)
    /// </summary>
    public Guid? ParentId { get; set; }

    [StringLength(CategoryConsts.MaxIconUrlLength)]
    public string? IconUrl { get; set; }

    [StringLength(CategoryConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    protected Category()
    {
    }

    public Category(Guid id, string name, string slug)
        : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), CategoryConsts.MaxNameLength);
        SetSlug(slug);
        IsActive = true;
    }

    public void SetSlug(string slug)
    {
        Slug = Check.NotNullOrWhiteSpace(slug, nameof(slug), CategoryConsts.MaxSlugLength);
    }
}
