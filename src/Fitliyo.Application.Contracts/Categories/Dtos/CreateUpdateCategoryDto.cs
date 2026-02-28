using System;
using System.ComponentModel.DataAnnotations;
using Fitliyo.Categories;

namespace Fitliyo.Categories.Dtos;

public class CreateUpdateCategoryDto
{
    [Required]
    [StringLength(CategoryConsts.MaxNameLength)]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(CategoryConsts.MaxSlugLength)]
    public string Slug { get; set; } = default!;

    public Guid? ParentId { get; set; }

    [StringLength(CategoryConsts.MaxIconUrlLength)]
    public string? IconUrl { get; set; }

    [StringLength(CategoryConsts.MaxDescriptionLength)]
    public string? Description { get; set; }

    public int SortOrder { get; set; }
}
