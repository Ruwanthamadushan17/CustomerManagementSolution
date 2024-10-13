using System.ComponentModel.DataAnnotations;

namespace CustomerAPI.DTOs
{
    public class CustomerRequestDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Address field is required.")]
        public string Address { get; set; }
    }
}
