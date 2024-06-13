namespace Nager.Authentication.Abstraction.Models
{
    /// <summary>
    /// Authentication Request
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// Email Address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// IpAddress
        /// </summary>
        public string? IpAddress { get; set; }
    }
}
