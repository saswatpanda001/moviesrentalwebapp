namespace MoviesRental.Models
{
    public class ReviewViewModel
    {
        public int MovieId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}