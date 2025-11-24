using System;
using System.Collections.Generic;

namespace MoviesRental.Models;

public partial class Rental
{
    public int RentalId { get; set; }

    public int UserId { get; set; }

    public int MovieId { get; set; }

    public DateTime RentedOn { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? ReturnedOn { get; set; }

    public decimal Price { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    // Add navigation property to Payment (optional since not all rentals may have payments)
    public virtual Payment? Payment { get; set; }
}