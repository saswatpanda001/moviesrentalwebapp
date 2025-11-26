using System;
using System.Collections.Generic;

namespace MoviesRental.Models;

using System;
using System.ComponentModel.DataAnnotations;

public partial class Review
{
    public int ReviewId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Rating is required.")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
    public int? Rating { get; set; }

    [MinLength(5, ErrorMessage = "Comment must be at least 5 characters long.")]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
