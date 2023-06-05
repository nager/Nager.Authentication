using Nager.Authentication.Abstraction.Models;

namespace Nager.Authentication.InMemoryRepository
{
    public class UserInfoWithPassword : UserInfo
    {
        public string Password { get; set; }
    }
}
