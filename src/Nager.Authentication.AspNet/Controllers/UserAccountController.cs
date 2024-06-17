using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.AspNet.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Controllers
{
    /// <summary>
    /// User Account Controller
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "useraccount")]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly IUserAccountService _userAccountService;

        /// <summary>
        /// User Account Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userAccountService"></param>
        public UserAccountController(
            ILogger<UserAccountController> logger,
            IUserAccountService userAccountService)
        {
            this._logger = logger;
            this._userAccountService = userAccountService;
        }

        /// <summary>
        /// Change Password of the logged in user
        /// </summary>
        /// <param name="changePasswordRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="204">Password changed</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [Route("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePasswordAsync(
            [Required][FromBody] ChangePasswordRequestDto changePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            var emailAddress = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(emailAddress) )
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var request = new UserChangePasswordRequest
            {
                Password = changePasswordRequest.Password,
            };

            if (await this._userAccountService.ChangePasswordAsync(emailAddress, request, cancellationToken))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Get Mfa Activation
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="204">Password changed</response>
        /// <response code="500">Unexpected error</response>
        [HttpGet]
        [Route("Mfa")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetMfaActivationAsync(
            CancellationToken cancellationToken = default)
        {
            var emailAddress = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(emailAddress))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var information = await this._userAccountService.GetMfaInformationAsync(emailAddress, cancellationToken);
            if (information == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return StatusCode(StatusCodes.Status200OK, information);
        }

        /// <summary>
        /// Activate Mfa
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="204">Mfa activated</response>
        /// <response code="400">Mfa workflow error</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [Route("Mfa/Activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MfaErrorResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActivateMfaAsync(
            [Required][FromBody] TimeBasedOneTimeTokenRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var emailAddress = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(emailAddress))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var mfaResult = await this._userAccountService.ActivateMfaAsync(emailAddress, request.Token, cancellationToken);

            switch (mfaResult)
            {
                case MfaActivationResult.Success:
                    return StatusCode(StatusCodes.Status204NoContent);

                case MfaActivationResult.UserNotFound:
                case MfaActivationResult.Failed:
                    return StatusCode(StatusCodes.Status500InternalServerError);

                case MfaActivationResult.AlreadyActive:
                case MfaActivationResult.InvalidCode:
                    return StatusCode(StatusCodes.Status400BadRequest, new MfaErrorResponseDto { Error = mfaResult.ToString() });

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }           
        }

        /// <summary>
        /// Deactivate Mfa
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="204">Mfa deactivated</response>
        /// <response code="400">Mfa workflow error</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [Route("Mfa/Deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(MfaErrorResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeactivateMfaAsync(
            [Required][FromBody] TimeBasedOneTimeTokenRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var emailAddress = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(emailAddress))
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var mfaResult = await this._userAccountService.DeactivateMfaAsync(emailAddress, request.Token, cancellationToken);

            switch (mfaResult)
            {
                case MfaDeactivationResult.Success:
                    return StatusCode(StatusCodes.Status204NoContent);

                case MfaDeactivationResult.UserNotFound:
                case MfaDeactivationResult.Failed:
                    return StatusCode(StatusCodes.Status500InternalServerError);

                case MfaDeactivationResult.NotActive:
                case MfaDeactivationResult.InvalidCode:
                    return StatusCode(StatusCodes.Status400BadRequest, new MfaErrorResponseDto { Error = mfaResult.ToString() });

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
