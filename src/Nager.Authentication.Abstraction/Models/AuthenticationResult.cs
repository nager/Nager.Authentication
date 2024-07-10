namespace Nager.Authentication.Abstraction.Models
{
    /// <summary>
    /// Authentication Result
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Authentication Status
        /// </summary>
        public AuthenticationStatus Status { get; set; }

        /// <summary>
        /// Mfa Identifier
        /// </summary>
        public string? MfaIdentifier { get; set; }
    }
}
