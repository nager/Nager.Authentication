using Nager.Authentication.Abstraction.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
{
    /// <summary>
    /// User Authentication Service Interface
    /// </summary>
    public interface IUserAuthenticationService
    {
        /// <summary>
        /// Validate Credentials
        /// </summary>
        /// <param name="authenticationRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<AuthenticationResult> ValidateCredentialsAsync(
            AuthenticationRequest authenticationRequest,
            CancellationToken cancellationToken = default);

        Task<ValidateTokenResult> ValidateTokenAsync(
            string mfaIdentifier,
            string token,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get UserInfo
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<UserInfo?> GetUserInfoAsync(
            string emailAddress,
            CancellationToken cancellationToken = default);
    }
}
