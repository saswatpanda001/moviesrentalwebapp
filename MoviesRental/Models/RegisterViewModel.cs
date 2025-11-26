using System.ComponentModel.DataAnnotations;

namespace MoviesRental.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Name must be at least 4 characters.")]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Email must be at least 4 characters.")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string? Phone { get; set; }

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;
    }
}