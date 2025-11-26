using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesRental.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required]
    [MinLength(4, ErrorMessage = "Name must be at least 4 characters long.")]
    public string Name { get; set; } = null!;


    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be exactly 10 characters.")]
    public string Phone { get; set; } = null!;

    [Required]
    [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be either 'Admin' or 'User'.")]
    public string? Role { get; set; } = "User";

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
