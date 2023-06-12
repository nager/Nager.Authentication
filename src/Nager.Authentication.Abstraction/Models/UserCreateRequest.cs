using System.ComponentModel.DataAnnotations;

namespace Nager.Authentication.Abstraction.Models
{
    public class UserCreateRequest
    {
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "The password is required")]
        public string Password { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
    }
}
