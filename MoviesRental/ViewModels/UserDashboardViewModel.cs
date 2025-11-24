using MoviesRental.Models;

namespace MoviesRental.ViewModels
{
    public class UserDashboardViewModel
    {
        public int ActiveRentals { get; set; }
        public int TotalReviews { get; set; }
        public int DueSoonRentals { get; set; }
        public int CartItemsCount { get; set; }
        public List<ActivityViewModel> RecentActivities { get; set; } = new();
        public List<DueSoonViewModel> DueSoonDetails { get; set; } = new();
    }

    public class ActivityViewModel
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class DueSoonViewModel
    {
        public string MovieTitle { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int DaysUntilDue { get; set; }
    }
}