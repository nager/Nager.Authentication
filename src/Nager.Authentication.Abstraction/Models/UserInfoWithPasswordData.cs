namespace Nager.Authentication.Abstraction.Models
{
    public class UserInfoWithPasswordData : UserInfo
    {
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
    }
}
