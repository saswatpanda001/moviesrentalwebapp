using System;
using System.Collections.Generic;

namespace MoviesRental.Models;

using System;
using System.ComponentModel.DataAnnotations;

public partial class Rental
{
    public int RentalId { get; set; }

    [Required(ErrorMessage = "User is required.")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Movie is required.")]
    public int MovieId { get; set; }

    [Required]
    public DateTime RentedOn { get; set; } = DateTime.Now;

    [Required]
    public DateTime DueDate { get; set; }


    public DateTime? ReturnedOn { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Price { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual User User { get; set; } = null!;


    public virtual Payment? Payment { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DueDate <= RentedOn)
        {
            yield return new ValidationResult(
                "DueDate must be after RentedOn.",
                new[] { nameof(DueDate), nameof(RentedOn) }
            );
        }

        if (ReturnedOn.HasValue && ReturnedOn < RentedOn)
        {
            yield return new ValidationResult(
                "ReturnedOn cannot be earlier than RentedOn.",
                new[] { nameof(ReturnedOn), nameof(RentedOn) }
            );
        }
    }


}
