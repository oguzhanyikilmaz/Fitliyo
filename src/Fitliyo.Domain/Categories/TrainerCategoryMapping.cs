using System;
using Volo.Abp.Domain.Entities;

namespace Fitliyo.Categories;

/// <summary>
/// Eğitmen ↔ Kategori çoka-çok ilişki tablosu
/// </summary>
public class TrainerCategoryMapping : Entity
{
    public Guid TrainerProfileId { get; set; }

    public Guid CategoryId { get; set; }

    protected TrainerCategoryMapping()
    {
    }

    public TrainerCategoryMapping(Guid trainerProfileId, Guid categoryId)
    {
        TrainerProfileId = trainerProfileId;
        CategoryId = categoryId;
    }

    public override object[] GetKeys()
    {
        return [TrainerProfileId, CategoryId];
    }
}
