using System;
using System.Collections.Generic;

namespace Fitliyo.Admin.Dtos;

public class PlatformStatsDto
{
    public List<DailyRevenueDto> DailyRevenue { get; set; } = [];
    public List<TopTrainerDto> TopTrainers { get; set; } = [];
    public List<CategoryStatsDto> CategoryStats { get; set; } = [];
}

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public decimal Commission { get; set; }
    public int OrderCount { get; set; }
}

public class TopTrainerDto
{
    public Guid TrainerProfileId { get; set; }
    public string? TrainerFullName { get; set; }
    public decimal TotalRevenue { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageRating { get; set; }
}

public class CategoryStatsDto
{
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int TrainerCount { get; set; }
    public int PackageCount { get; set; }
}
