namespace Fitliyo.Admin.Dtos;

public class DashboardDto
{
    public int TotalTrainers { get; set; }
    public int ActiveTrainers { get; set; }
    public int VerifiedTrainers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCommission { get; set; }
    public int TotalReviews { get; set; }
    public decimal AveragePlatformRating { get; set; }
    public int TotalActiveSubscriptions { get; set; }
    public int TotalPackages { get; set; }
    public int TotalCategories { get; set; }
}
