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

        public Task<bool> ChangePasswordAsync(
            string emailAddress,
            UserChangePasswordRequest userChangePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            //this._userRepository.ChangePasswordAsync()

            throw new System.NotImplementedException();
        }
    }
}
