using System.ComponentModel.DataAnnotations;

namespace CustomerUI.Models
{
    public class CustomerViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The First Name field is required.")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "Address must be at least 1 character long.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required.")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "Address must be at least 1 character long.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Email must not exceed 100 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Address field is required.")]
        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "Address must be at least 1 character long.")]
        public string Address { get; set; }

        [StringLength(15, ErrorMessage = "Mobile Number must not exceed 15 characters.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid Mobile Number.")]
        public string? MobileNo { get; set; }
    }
}
