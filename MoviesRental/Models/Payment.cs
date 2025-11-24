using System;

namespace MoviesRental.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }

        public int RentalId { get; set; }

        public int UserId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = null!;   // "Card", "UPI", "Cash"

        public string Status { get; set; } = "Completed";    // "Completed", "Pending", "Failed"

        public DateTime PaidOn { get; set; }

        public virtual Rental Rental { get; set; } = null!;

        public virtual User User { get; set; } = null!;
    }
}