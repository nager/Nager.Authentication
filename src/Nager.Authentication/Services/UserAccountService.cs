using Google.Authenticator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.Abstraction.Validators;
using Nager.Authentication.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Services
{
    /// <summary>
    /// UserAccount Service
    /// </summary>
    public class UserAccountService : IUserAccountService
    {
        private readonly ILogger<UserAccountService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly string _issuer;

        public UserAccountService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<UserAccountService>? logger = default)
        {
            this._logger = logger ?? new NullLogger<UserAccountService>();

            this._userRepository = userRepository;
            this._issuer = configuration["Authentication:Tokens:Issuer"];
        }

        /// <inheritdoc />
        public async Task<bool> ChangePasswordAsync(
            string emailAddress,
            UserChangePasswordRequest userUpdatePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            if (userEntity == null)
            {
                return false;
            }

            var passwordHash = PasswordHelper.HashPasword(userUpdatePasswordRequest.Password, userEntity.PasswordSalt);
            userEntity.PasswordHash = passwordHash;

            await this._userRepository.UpdateAsync(userEntity);

            return true;
        }

        /// <inheritdoc />
        public async Task<string?> GetMfaActivationQrCodeAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            if (userEntity == null)
            {
                return null;
            }

            if (userEntity.mfaSecret == null)
            {
                userEntity.mfaSecret = PasswordHelper.CreateSalt();
                if (!await this._userRepository.UpdateAsync(userEntity))
                {
                    this._logger.LogError("Cannot update mfaSecret");
                    return null;
                }
            }

            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            var setupCode = twoFactorAuthenticator.GenerateSetupCode(this._issuer, emailAddress, userEntity.mfaSecret);

            return setupCode.QrCodeSetupImageUrl;
        }

        /// <inheritdoc />
        public async Task<bool> ActivateMfaAsync(
            string emailAddress,
            string token,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            if (userEntity == null)
            {
                return false;
            }

            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            if (twoFactorAuthenticator.ValidateTwoFactorPIN(userEntity.mfaSecret, token))
            {
                userEntity.mfaActive = true;
                return await this._userRepository.UpdateAsync(userEntity);
            }

            return false;
        }
    }
}
