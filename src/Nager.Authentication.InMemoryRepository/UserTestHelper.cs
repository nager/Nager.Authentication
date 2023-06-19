using Nager.Authentication.Abstraction.Services;
using System.Threading.Tasks;

namespace Nager.Authentication.InMemoryRepository
{
    public static class UserTestHelper
    {
        public static async Task CreateAsync(UserInfoWithPassword[] items, IUserManagementService userManagementService)
        {
            foreach (var item in items)
            {
                await userManagementService.CreateAsync(new Abstraction.Models.UserCreateRequest
                {
                    EmailAddress = item.EmailAddress,
                    Password = item.Password,
                    Firstname = item.Firstname,
                    Lastname = item.Lastname,
                    Roles = item.Roles
                });
            }
        }
    }
}
