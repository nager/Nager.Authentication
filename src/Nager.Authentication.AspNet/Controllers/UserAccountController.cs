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
    [ApiController]
    [ApiExplorerSettings(GroupName = "useraccount")]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class UserAccountController : ControllerBase
    {
        private readonly ILogger<UserAccountController> _logger;
        private readonly IUserAccountService _userAccountService;

        public UserAccountController(
            ILogger<UserAccountController> logger,
            IUserAccountService userAccountService)
        {
            this._logger = logger;
            this._userAccountService = userAccountService;
        }

        /// <summary>
        /// ChangePassword
        /// </summary>
        /// <param name="changePasswordRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<ActionResult> ChangePasswordAsync(
            [Required][FromBody] ChangePasswordRequestDto changePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            var emailAddress = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(emailAddress) )
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var request = new UserUpdatePasswordRequest
            {
                Password = changePasswordRequest.Password,
            };

            if (await this._userAccountService.ChangePasswordAsync(emailAddress, request, cancellationToken))
            {
                return StatusCode(StatusCodes.Status200OK);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
