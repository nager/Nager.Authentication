using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using System.Threading.Tasks;

namespace Nager.Authentication.InMemoryRepository
{
    public static class InitialUserHelper
    {
        public static async Task CreateAsync(
            UserInfoWithPassword[] items,
            IUserManagementService userManagementService)
        {
            foreach (var item in items)
            {
                var userInfo = await userManagementService.GetByEmailAddressAsync(item.EmailAddress);
                if (userInfo != null)
                {
                    continue;
                }

                var createRequest = new UserCreateRequest
                {
                    EmailAddress = item.EmailAddress,
                    Password = item.Password,
                    Firstname = item.Firstname,
                    Lastname = item.Lastname,
                    Roles = item.Roles
                };

                var successful = await userManagementService.CreateAsync(createRequest);
                if (!successful)
                {
                    //log failure
                }
            }
        }
    }
}
