using System.ComponentModel.DataAnnotations;

namespace Nager.Authentication.AspNet.Dtos
{
    public class AuthenticationMfaTokenRequestDto
    {
        [Required]
        public string MfaIdentifier { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
