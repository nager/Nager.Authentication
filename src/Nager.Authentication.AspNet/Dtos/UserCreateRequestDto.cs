using System.ComponentModel.DataAnnotations;

namespace Nager.Authentication.AspNet.Dtos
{
    public class UserCreateRequestDto
    {
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "The password is required")]
        public string Password { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }
    }
}
