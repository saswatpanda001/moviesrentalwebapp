using System.ComponentModel.DataAnnotations;
using System;


namespace MoviesRental.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public virtual User User { get; set; } = null!;
    }

    public class CartItem
    {
        public int CartItemId { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Range(1, 10, ErrorMessage = "Quantity must be at least 1 and max 10.")]
        public int Quantity { get; set; } = 1;

        [Range(1, 30, ErrorMessage = "Rental days must be between 1 and 30.")]
        public int RentalDays { get; set; } = 7;

        public DateTime AddedAt { get; set; } = DateTime.Now;

        public virtual Cart Cart { get; set; } = null!;
        public virtual Movie Movie { get; set; } = null!;
    }
}