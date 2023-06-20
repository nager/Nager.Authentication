//using System.ComponentModel.DataAnnotations;

namespace Nager.Authentication.Abstraction.Entities
{
    public class UserEntity
    {
        //[MaxLength(200)]
        public string Id { get; set; }

        //[MaxLength(200)]
        public string EmailAddress { get; set; }

        public string RolesData { get; set; }

        //[MaxLength(200)]
        public string Firstname { get; set; }

        //[MaxLength(200)]
        public string Lastname { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set;}
    }
}
