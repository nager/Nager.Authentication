namespace Nager.Authentication.Abstraction.Models
{
    public class AuthenticationResult
    {
        public AuthenticationStatus Status { get; set; }
        public string MfaIdentifier { get; set; }
    }
}
