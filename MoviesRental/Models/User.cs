using System;
using System.Collections.Generic;

namespace MoviesRental.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Role { get; set; } = "User";

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
