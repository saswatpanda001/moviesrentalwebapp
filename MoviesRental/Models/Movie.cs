using System;
using System.Collections.Generic;

namespace MoviesRental.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public string? Genre { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Description { get; set; }

    public int Stock { get; set; }

    public decimal RentalPrice { get; set; }

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
