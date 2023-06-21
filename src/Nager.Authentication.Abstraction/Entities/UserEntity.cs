using System.ComponentModel.DataAnnotations;

namespace Nager.Authentication.Abstraction.Entities
{
    public class UserEntity
    {
        [StringLength(200)]
        public string Id { get; set; }

        [MaxLength(200)]
        public string EmailAddress { get; set; }

        [MaxLength(200)]
        public string? Firstname { get; set; }

        [MaxLength(200)]
        public string? Lastname { get; set; }

        [MaxLength(1000)]
        public string? RolesData { get; set; }

        [MaxLength(16)]
        public byte[] PasswordSalt { get; set;}

        [MaxLength(32)]
        public byte[]? PasswordHash { get; set; }
    }
}
