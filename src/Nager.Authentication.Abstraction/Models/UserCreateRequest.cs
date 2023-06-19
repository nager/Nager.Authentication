namespace Nager.Authentication.Abstraction.Models
{
    public class UserCreateRequest
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string[] Roles { get; set; }
    }
}
