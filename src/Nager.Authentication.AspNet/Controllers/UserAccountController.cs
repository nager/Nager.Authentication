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
        [Route("MfaActivationInfo")]
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

            var image = await this._userAccountService.GetMfaActivationQrCodeAsync(emailAddress, cancellationToken);

            return StatusCode(StatusCodes.Status200OK, new { image = image});
        }

        /// <summary>
        /// Activate mfa
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="204">Password changed</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [Route("MfaActivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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

            if (await this._userAccountService.ActivateMfaAsync(emailAddress, request.Token, cancellationToken))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
