using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nager.Authentication.Abstraction.Helpers;
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
            UserUpdatePasswordRequest userUpdatePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress));
            if (userEntity == null)
            {
                return false;
            }

            var passwordHash = PasswordHelper.HashPasword(userUpdatePasswordRequest.Password, userEntity.PasswordSalt);
            userEntity.PasswordHash = passwordHash;

            await this._userRepository.UpdateAsync(userEntity);

            return true;
        }
    }
}
