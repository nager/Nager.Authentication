using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.AspNet.Dtos;
using Nager.Authentication.AspNet.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Controllers
{
    /// <summary>
    /// Authentication Controller
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(GroupName = "authentication")]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserAuthenticationService _userAuthenticationService;

        /// <summary>
        /// Authentication Controller
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        /// <param name="userAuthenticationService"></param>
        public AuthenticationController(
            ILogger<AuthenticationController> logger,
            IConfiguration configuration,
            IUserAuthenticationService userAuthenticationService)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._userAuthenticationService = userAuthenticationService;
        }

        private async Task<JwtSecurityToken> CreateTokenAsync(
            string emailAddress)
        {
            var issuer = this._configuration["Authentication:Tokens:Issuer"];
            var audience = this._configuration["Authentication:Tokens:Audience"];
            var signingKey = this._configuration["Authentication:Tokens:SigningKey"];

            //TODO: load from config
            var expiresAt = DateTime.UtcNow.AddDays(7);

            if (string.IsNullOrEmpty(issuer))
            {
                throw new MissingConfigurationException($"{nameof(issuer)} is missing");
            }

            if (string.IsNullOrEmpty(signingKey))
            {
                throw new MissingConfigurationException($"{nameof(signingKey)} is missing");
            }

            var userInfo = await this._userAuthenticationService.GetUserInfoAsync(emailAddress);
            if (userInfo == null)
            {
                throw new UnknownUserException();
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, emailAddress),
                new(JwtRegisteredClaimNames.Email, emailAddress)
            };


            if (!string.IsNullOrEmpty(userInfo.Firstname))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, userInfo.Firstname));
            }

            if (!string.IsNullOrEmpty(userInfo.Lastname))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.FamilyName, userInfo.Lastname));
            }

            if (userInfo.Roles != null)
            {
                foreach (var role in userInfo.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(issuer,
                audience,
                claims,
                expires: expiresAt,
                signingCredentials: credentials);
        }

        /// <summary>
        /// Authenticate via Email and Password
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Authentication successful</response>
        /// <response code="401">Mfa required</response>
        /// <response code="406">Invalid credential</response>
        /// <response code="429">Credential check temporarily locked</response>
        /// <response code="500">Unexpected error</response>
        [AllowAnonymous]
        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthenticationResponseDto>> AuthenticateAsync(
            [Required][FromBody] AuthenticationRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var ipAddress = HttpContext.GetIpAddress();
            this._logger.LogInformation($"{nameof(AuthenticateAsync)} - New request from: {ipAddress} {request.EmailAddress}");

            var authenticationRequest = new AuthenticationRequest
            {
                EmailAddress = request.EmailAddress,
                Password = request.Password,
                IpAddress = ipAddress
            };

            var authenticationResult = await this._userAuthenticationService.ValidateCredentialsAsync(authenticationRequest, cancellationToken);
            this._logger.LogInformation($"{nameof(AuthenticateAsync)} - EmailAddress:{request.EmailAddress}, AuthenticationStatus:{authenticationResult}");

            var tokenHandler = new JwtSecurityTokenHandler();

            switch (authenticationResult.Status)
            {
                case AuthenticationStatus.Invalid:
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                case AuthenticationStatus.Valid:
                    try
                    {
                        var jwtSecurityToken = await this.CreateTokenAsync(request.EmailAddress);
                        var token = tokenHandler.WriteToken(jwtSecurityToken);

                        return StatusCode(StatusCodes.Status200OK, new AuthenticationResponseDto
                        {
                            Token = token,
                            Expiration = jwtSecurityToken.ValidTo
                        });
                    }
                    catch (Exception exception)
                    {
                        this._logger.LogError(exception, $"{nameof(AuthenticateAsync)}");
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                case AuthenticationStatus.MfaCodeRequired:
                    if (string.IsNullOrEmpty(authenticationResult.MfaIdentifier))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }

                    return StatusCode(StatusCodes.Status401Unauthorized, new MfaRequiredResponseDto
                    {
                        MfaType = "TOTP",
                        MfaIdentifier = authenticationResult.MfaIdentifier
                    });
                case AuthenticationStatus.TemporaryBlocked:
                    return StatusCode(StatusCodes.Status429TooManyRequests);
                default:
                    return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }

        /// <summary>
        /// Authenticate Mfa Token
        /// </summary>
        /// <remarks>Second Step Time-based one-time password</remarks>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <response code="200">Authentication successful</response>
        /// <response code="400">Invalid token</response>
        /// <response code="500">Unexpected error</response>
        [AllowAnonymous]
        [HttpPost]
        [Route("Token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthenticationResponseDto>> AuthenticateMfaTokenAsync(
            [Required][FromBody] AuthenticationMfaTokenRequestDto request,
            CancellationToken cancellationToken = default)
        {
            var validateTokenResult = await this._userAuthenticationService.ValidateTokenAsync(request.MfaIdentifier, request.Token, cancellationToken);
            if (!validateTokenResult.Success)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            var jwtSecurityToken = await this.CreateTokenAsync(validateTokenResult.EmailAddress);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(jwtSecurityToken);

            return StatusCode(StatusCodes.Status200OK, new AuthenticationResponseDto
            {
                Token = token,
                Expiration = jwtSecurityToken.ValidTo
            });
        }

        /// <summary>
        /// Validate Authentication
        /// </summary>
        /// <returns></returns>
        /// <response code="204">Token valid</response>
        /// <response code="401">Token invalid</response>
        /// <response code="500">Unexpected error</response>
        [HttpPost]
        [Route("Validate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<AuthenticationResponseDto> Validate()
        {
            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
