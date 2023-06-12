using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Validators;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly ILogger<UserAccountService> _logger;

        private readonly IUserRepository _userRepository;

        public UserAccountService(
            IUserRepository userRepository,
            ILogger<UserAccountService>? logger = default)
        {
            this._logger = logger ?? new NullLogger<UserAccountService>();

            this._userRepository = userRepository;
        }

        public async Task<bool> ChangePasswordAsync(
            string emailAddress,
            UserUpdatePasswordRequest userChangePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            var userInfo = await this._userRepository.GetUserInfoAsync(emailAddress);
            if (userInfo == null)
            {
                return false;
            }

            return await this._userRepository.UpdatePasswordAsync(userInfo.Id, new UserUpdatePasswordRequest
            {
                Password = userChangePasswordRequest.Password
            });
        }
    }
}
