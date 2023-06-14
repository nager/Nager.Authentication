using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.AspNet.Dtos;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Controllers
{
    /// <summary>
    /// User Management Controller
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "usermanagement")]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly ILogger<UserManagementController> _logger;
        private readonly IUserManagementService _userManagementService;

        /// <summary>
        /// User Management Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="userManagementService"></param>
        public UserManagementController(
            ILogger<UserManagementController> logger,
            IUserManagementService userManagementService)
        {
            this._logger = logger;
            this._userManagementService = userManagementService;
        }

        /// <summary>
        /// Get user by given id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "administrator")]
        [Route("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserInfoDto>> GetUserAsync(
            [FromRoute] string userId,
            CancellationToken cancellationToken = default)
        {
            var userInfo = await this._userManagementService.GetAsync(userId, cancellationToken);
            if (userInfo == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var item = new UserInfoDto
            {
                Id = userInfo.Id,
                EmailAddress = userInfo.EmailAddress,
                Firstname = userInfo.Firstname,
                Lastname = userInfo.Lastname,
                Roles = userInfo.Roles
            };

            return StatusCode(StatusCodes.Status200OK, item);
        }

        /// <summary>
        /// Query users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "administrator")]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserInfoDto[]>> QueryUsersAsync(
            [FromQuery] int take = 100,
            [FromQuery] int skip = 0,
            CancellationToken cancellationToken = default)
        {
            var userInfos = await this._userManagementService.QueryAsync(take, skip, cancellationToken);

            var items = userInfos.Select(userInfo => new UserInfoDto
            {
                Id = userInfo.Id,
                EmailAddress = userInfo.EmailAddress,
                Firstname = userInfo.Firstname,
                Lastname = userInfo.Lastname,
                Roles = userInfo.Roles
            });

            return StatusCode(StatusCodes.Status200OK, items);
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "administrator")]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> AddUserAsync(
            [FromBody] UserCreateRequestDto createRequest,
            CancellationToken cancellationToken = default)
        {
            var createUserRequest = new UserCreateRequest
            {
                EmailAddress = createRequest.EmailAddress,
                Password = createRequest.Password,
                Firstname = createRequest.Firstname,
                Lastname = createRequest.Lastname
            };

            if (await this._userManagementService.CreateAsync(createUserRequest, cancellationToken))
            {
                return StatusCode(StatusCodes.Status201Created);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Edit user by given id
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "administrator")]
        [Route("{userid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> EditUserAsync(
            [FromRoute] string userId,
            [FromBody] UserUpdateNameRequestDto updateRequest,
            CancellationToken cancellationToken = default)
        {
            var updateUserRequest = new UserUpdateNameRequest
            {
                Firstname = updateRequest.Firstname,
                Lastname = updateRequest.Lastname
            };

            if (await this._userManagementService.UpdateAsync(userId, updateUserRequest, cancellationToken))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Delete user by given id
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "administrator")]
        [Route("{userid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteUserAsync(
             [FromRoute] string userId,
             CancellationToken cancellationToken = default)
        {
            if (await this._userManagementService.DeleteAsync(userId, cancellationToken))
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
