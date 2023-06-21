using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.Abstraction.Validators;
using Nager.Authentication.InMemoryRepository;
using System.Threading.Tasks;

namespace Nager.Authentication.UnitTest
{
    [TestClass]
    public class UserServiceTest
    {
        [TestMethod]
        public async Task ValidateCredentialsAsync_10Retrys_Block()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var userInfos = new UserInfoWithPassword[]
            {
                new UserInfoWithPassword
                {
                    EmailAddress = "admin",
                    Password = "secret",
                    Roles = new [] { "Administrator" }
                }
            };

            IUserRepository userRepository = new InMemoryUserRepository();

            IUserManagementService userManagementService = new UserManagementService(userRepository);
            IUserAuthenticationService userService = new UserAuthenticationService(userRepository, memoryCache);

            await InitialUserHelper.CreateAsync(userInfos, userManagementService);

            var authenticationRequest = new AuthenticationRequest
            {
                IpAddress = "1.2.3.4",
                EmailAddress = "admin",
                Password = "test"
            };

            for (var i = 0; i <= 10; i++)
            {
                var authenticationStatus = await userService.ValidateCredentialsAsync(authenticationRequest);
                Assert.AreEqual(AuthenticationStatus.Invalid, authenticationStatus, $"Retry: {i}");
            }

            var authenticationStatus1 = await userService.ValidateCredentialsAsync(authenticationRequest);
            Assert.AreEqual(AuthenticationStatus.TemporaryBlocked, authenticationStatus1);
        }

        [TestMethod]
        public async Task ValidateCredentialsAsync_UpperCaseUsername_Allow()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var userInfos = new UserInfoWithPassword[]
            {
                new UserInfoWithPassword
                {
                    EmailAddress = "admin",
                    Password = "secret",
                    Roles = new [] { "Administrator" }
                }
            };

            
            IUserRepository userRepository = new InMemoryUserRepository();

            IUserManagementService userManagementService = new UserManagementService(userRepository);
            IUserAuthenticationService userService = new UserAuthenticationService(userRepository, memoryCache);

            await InitialUserHelper.CreateAsync(userInfos, userManagementService);

            var authenticationRequest = new AuthenticationRequest
            {
                IpAddress = "1.2.3.4",
                EmailAddress = "Admin",
                Password = "secret"
            };

            var authenticationStatus = await userService.ValidateCredentialsAsync(authenticationRequest);
            Assert.AreEqual(AuthenticationStatus.Valid, authenticationStatus);
        }
    }
}
