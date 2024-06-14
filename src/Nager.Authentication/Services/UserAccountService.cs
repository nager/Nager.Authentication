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
    /// User Account Service
    /// </summary>
    public class UserAccountService : IUserAccountService
    {
        private readonly ILogger<UserAccountService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly string _issuer;

        /// <summary>
        /// User Account Service
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
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

            await this._userRepository.UpdateAsync(userEntity, cancellationToken);

            return true;
        }

        private async Task<bool> CreateMfaSecretAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            if (userEntity == null)
            {
                return false;
            }

            if (userEntity.MfaSecret == null)
            {
                userEntity.MfaSecret = ByteHelper.CreatePseudoRandomNumber();
                return await this._userRepository.UpdateAsync(userEntity, cancellationToken);
            }

            return true;
        }

        /// <inheritdoc />
        public async Task<MfaActivationResult> ActivateMfaAsync(
            string emailAddress,
            string token,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            if (userEntity == null)
            {
                return MfaActivationResult.UserNotFound;
            }

            if (userEntity.MfaActive)
            {
                return MfaActivationResult.AlreadyActive;
            }

            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            if (twoFactorAuthenticator.ValidateTwoFactorPIN(userEntity.MfaSecret, token))
            {
                userEntity.MfaActive = true;
                if (await this._userRepository.UpdateAsync(userEntity, cancellationToken))
                {
                    return MfaActivationResult.Success;
                }

                return MfaActivationResult.Failed;
            }

            return MfaActivationResult.InvalidCode;
        }

        /// <inheritdoc />
        public async Task<MfaDeactivationResult> DeactivateMfaAsync(
            string emailAddress,
            string token,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            if (userEntity == null)
            {
                return MfaDeactivationResult.UserNotFound;
            }

            if (!userEntity.MfaActive)
            {
                return MfaDeactivationResult.NotActive;
            }

            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            if (twoFactorAuthenticator.ValidateTwoFactorPIN(userEntity.MfaSecret, token))
            {
                userEntity.MfaActive = false;
                userEntity.MfaSecret = null;

                if (await this._userRepository.UpdateAsync(userEntity, cancellationToken))
                {
                    return MfaDeactivationResult.Success;
                }

                return MfaDeactivationResult.Failed;
            }

            return MfaDeactivationResult.InvalidCode;
        }

        public async Task<MfaInformation?> GetMfaInformationAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            if (userEntity == null)
            {
                return null;
            }

            if (userEntity.MfaActive)
            {
                return new MfaInformation
                {
                    IsActive = true
                };
            }

            if (userEntity.MfaSecret == null)
            {
                if (!await this.CreateMfaSecretAsync(emailAddress, cancellationToken))
                {
                    return null;
                }

                userEntity = await this._userRepository.GetAsync(o => o.EmailAddress.Equals(emailAddress), cancellationToken);
            }

            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            var setupCode = twoFactorAuthenticator.GenerateSetupCode(this._issuer, emailAddress, userEntity.MfaSecret);

            return new MfaInformation
            {
                IsActive = false,
                ActivationQrCode = setupCode.QrCodeSetupImageUrl
            };
        }
    }
}
