namespace Nager.Authentication.Abstraction.Models
{
    public class AuthenticationRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string? IpAddress { get; set; }
    }
}
