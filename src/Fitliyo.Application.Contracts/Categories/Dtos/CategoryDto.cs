using System;
using Volo.Abp.Application.Dtos;

namespace Fitliyo.Categories.Dtos;

public class CategoryDto : EntityDto<Guid>
{
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public Guid? ParentId { get; set; }
    public string? IconUrl { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}
