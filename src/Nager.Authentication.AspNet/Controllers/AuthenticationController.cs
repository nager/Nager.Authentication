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
    [ApiController]
    [ApiExplorerSettings(GroupName = "authentication")]
    [Authorize]
    [Route("api/v1/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserAuthenticationService _userAuthenticationService;

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
            AuthenticationRequestDto request)
        {
            var issuer = this._configuration["Authentication:Tokens:Issuer"];
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

            var roles = await this._userAuthenticationService.GetRolesAsync(request.EmailAddress);
            if (roles == null)
            {
                throw new UnknownUserException();
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, request.EmailAddress),
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    //TODO: Check role with schema is a good chooise
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(issuer,
                issuer,
                claims,
                expires: expiresAt,
                signingCredentials: credentials);
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("")]
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

            var authenticationStatus = await this._userAuthenticationService.ValidateCredentialsAsync(authenticationRequest, cancellationToken);
            this._logger.LogInformation($"{nameof(AuthenticateAsync)} - EmailAddress:{request.EmailAddress} {authenticationStatus}");

            switch (authenticationStatus)
            {
                case AuthenticationStatus.Invalid:
                    return StatusCode(StatusCodes.Status406NotAcceptable);
                case AuthenticationStatus.Valid:
                    try
                    {
                        var jwtSecurityToken = await this.CreateTokenAsync(request);
                        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

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
                case AuthenticationStatus.TemporaryBlocked:
                    return StatusCode(StatusCodes.Status429TooManyRequests);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
