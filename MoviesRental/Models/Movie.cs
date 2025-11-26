using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesRental.Models
{
    public partial class Movie
    {
        public int MovieId { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Title must be at least 4 characters long.")]
        public string Title { get; set; } = null!;

        [MinLength(4, ErrorMessage = "Genre must be at least 4 characters long.")]
        public string? Genre { get; set; }

        [Range(1801, 2025, ErrorMessage = "Release year must be between 1801 and 2025.")]
        public int? ReleaseYear { get; set; }


        [Required]
        [MinLength(6, ErrorMessage = "Desc must be at least 6 characters long.")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Rental price must be greater than zero.")]
        public decimal RentalPrice { get; set; }

        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
