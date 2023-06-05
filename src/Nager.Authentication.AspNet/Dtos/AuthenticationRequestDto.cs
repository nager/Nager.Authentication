using System.ComponentModel.DataAnnotations;

namespace Nager.Authentication.AspNet.Dtos
{
    public class AuthenticationRequestDto
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
