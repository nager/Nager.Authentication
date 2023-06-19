namespace Nager.Authentication.Abstraction.Entities
{
    public class UserEntity
    {
        public string Id { get; set; }

        public string EmailAddress { get; set; }

        public string[] Roles { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set;}
    }
}
