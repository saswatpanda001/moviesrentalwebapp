namespace MoviesRental.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual User User { get; set; } = null!;
    }

    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int MovieId { get; set; }
        public int Quantity { get; set; } = 1;
        public int RentalDays { get; set; } = 7;
        public DateTime AddedAt { get; set; } = DateTime.Now;

        public virtual Cart Cart { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;
    }
}