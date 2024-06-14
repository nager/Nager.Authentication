using Nager.Authentication.Abstraction.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
{
    /// <summary>
    /// User Account Service Interface
    /// </summary>
    public interface IUserAccountService
    {
        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="userChangePasswordRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ChangePasswordAsync(
            string emailAddress,
            UserChangePasswordRequest userChangePasswordRequest,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get Mfa Information
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MfaInformation> GetMfaInformationAsync(
            string emailAddress,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Activate Mfa
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MfaActivationResult> ActivateMfaAsync(
            string emailAddress,
            string token,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivate Mfa
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MfaDeactivationResult> DeactivateMfaAsync(
            string emailAddress,
            string token,
            CancellationToken cancellationToken = default);
    }
}
