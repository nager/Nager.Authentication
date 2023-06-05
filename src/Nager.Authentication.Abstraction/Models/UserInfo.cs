namespace Nager.Authentication.Abstraction.Models
{
    public class UserInfo
    {
        public string Id { get; set; }

        public string EmailAddress { get; set; }

        public string[] Roles { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }
    }
}
