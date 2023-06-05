using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "usermanagement")]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(
            ILogger<UserManagementController> logger,
            IUserManagementService userManagementService)
        {
            this._logger = logger;
            this._userManagementService = userManagementService;
        }

        /// <summary>
        /// Get user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "administrator")]
        [Route("{userId}")]
        public async Task<ActionResult> GetUserAsync(
            [FromRoute] string userId,
            CancellationToken cancellationToken = default)
        {
            var userInfo = await this._userManagementService.GetAsync(userId, cancellationToken);

            //TODO: Dto Model?
            return StatusCode(StatusCodes.Status200OK, userInfo);
        }

        /// <summary>
        /// Query users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "administrator")]
        [Route("")]
        public async Task<ActionResult> QueryUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var userInfos = await this._userManagementService.QueryAsync(100, 0, cancellationToken);

            return StatusCode(StatusCodes.Status200OK, userInfos);
        }

        /// <summary>
        /// Add new user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "administrator")]
        [Route("")]
        public async Task<ActionResult> AddUserAsync(
            [FromBody] UserCreateRequest createRequest,
            CancellationToken cancellationToken = default)
        {
            if (await this._userManagementService.CreateAsync(createRequest, cancellationToken))
            {
                return StatusCode(StatusCodes.Status200OK);
            }

            return StatusCode(StatusCodes.Status200OK);
        }

        /// <summary>
        /// Edit user
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "administrator")]
        [Route("{userid}")]
        public async Task<ActionResult> EditUserAsync(
            [FromRoute] string userId,
            [FromBody] UserUpdateRequest updateRequest,
            CancellationToken cancellationToken = default)
        {
            if (await this._userManagementService.UpdateAsync(userId, updateRequest, cancellationToken))
            {
                return StatusCode(StatusCodes.Status200OK);
            }

            return StatusCode(StatusCodes.Status200OK);
        }

        /// <summary>
        /// Delete user
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "administrator")]
        [Route("{userid}")]
        public async Task<ActionResult> DeleteUserAsync(
             [FromRoute] string userId,
             CancellationToken cancellationToken = default)
        {
            if (await this._userManagementService.DeleteAsync(userId, cancellationToken))
            {
                return StatusCode(StatusCodes.Status200OK);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
