using Microsoft.Extensions.Caching.Memory;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Services;
using Nager.Authentication.Abstraction.Validators;
using Nager.Authentication.Helpers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Services
{
    /// <summary>
    /// User Authentication Service
    /// </summary>
    /// <remarks>With Brute-Force Protection</remarks>
    public class UserAuthenticationService : IUserAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly string _cacheKeyPrefix = "AuthenticationInfo";
        private readonly TimeSpan _cacheLiveTime = TimeSpan.FromMinutes(10);
        private readonly int _delayTimeMultiplier = 100;
        private readonly int _maxInvalidLogins = 10;
        private readonly int _maxInvalidLoginsBeforeDelay = 3;

        public UserAuthenticationService(
            IUserRepository userRepository,
            IMemoryCache memoryCache)
        {
            this._userRepository = userRepository;
            this._memoryCache = memoryCache;
        }

        private string GetCacheKey(string ipAddress)
        {
            return $"{this._cacheKeyPrefix}.{ipAddress}";
        }

        private void SetInvalidLogin(string ipAddress)
        {
            var cacheKey = this.GetCacheKey(ipAddress);
            if (!this._memoryCache.TryGetValue<AuthenticationInfo>(cacheKey, out var authenticationInfo))
            {
                authenticationInfo = new AuthenticationInfo();
            }

            if (authenticationInfo == null)
            {
                throw new ArgumentNullException(nameof(authenticationInfo));
            }

            authenticationInfo.InvalidCount++;
            authenticationInfo.LastInvalid = DateTime.Now;

            this._memoryCache.Set(cacheKey, authenticationInfo, _cacheLiveTime);
        }

        private void SetValidLogin(string ipAddress)
        {
            var cacheKey = this.GetCacheKey(ipAddress);
            if (!this._memoryCache.TryGetValue<AuthenticationInfo>(cacheKey, out var authenticationInfo))
            {
                authenticationInfo = new AuthenticationInfo();
            }

            if (authenticationInfo == null)
            {
                throw new ArgumentNullException(nameof(authenticationInfo));
            }

            authenticationInfo.LastValid = DateTime.Now;
            authenticationInfo.InvalidCount = 0;

            this._memoryCache.Set(cacheKey, authenticationInfo, this._cacheLiveTime);
        }

        private async Task<bool> IsIpAddressBlockedAsync(string ipAddress)
        {
            var cacheKey = this.GetCacheKey(ipAddress);

            if (!this._memoryCache.TryGetValue<AuthenticationInfo>(cacheKey, out var authenticationInfo))
            {
                return false;
            }

            if (authenticationInfo == null)
            {
                throw new NullReferenceException(nameof(authenticationInfo));
            }

            if (authenticationInfo.InvalidCount < this._maxInvalidLoginsBeforeDelay)
            {
                return false;
            }

            await Task.Delay(authenticationInfo.InvalidCount * this._delayTimeMultiplier);

            if (authenticationInfo.InvalidCount > this._maxInvalidLogins && DateTime.Now < authenticationInfo.LastInvalid.AddMinutes(2))
            {
                return true;
            }

            return false;
        }

        public async Task<AuthenticationStatus> ValidateCredentialsAsync(
            AuthenticationRequest authenticationRequest,
            CancellationToken cancellationToken = default)
        {
            if (authenticationRequest == null)
            {
                throw new ArgumentNullException(nameof(authenticationRequest));
            }

            if (string.IsNullOrEmpty(authenticationRequest.IpAddress))
            {
                throw new NullReferenceException(nameof(authenticationRequest.IpAddress));
            }

            if (await this.IsIpAddressBlockedAsync(authenticationRequest.IpAddress))
            {
                return AuthenticationStatus.TemporaryBlocked;
            }

            //TODO: Protect users when trying to flood the same user
            //      with requests from different IP addresses in a short period of time
            //      add cache item with username

            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == authenticationRequest.EmailAddress, cancellationToken);
            if (userEntity == null)
            {
                this.SetInvalidLogin(authenticationRequest.IpAddress);
                return AuthenticationStatus.Invalid;
            }

            if (userEntity.PasswordHash == null)
            {
                throw new NullReferenceException(nameof(userEntity.PasswordHash));
            }

            var passwordHash = PasswordHelper.HashPasword(authenticationRequest.Password, userEntity.PasswordSalt);
            if (userEntity.PasswordHash.SequenceEqual(passwordHash))
            {
                //Set Last Login Time

                this.SetValidLogin(authenticationRequest.IpAddress);
                return AuthenticationStatus.Valid;
            }

            this.SetInvalidLogin(authenticationRequest.IpAddress);
            return AuthenticationStatus.Invalid;
        }

        public async Task<UserInfo?> GetUserInfoAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == emailAddress);

            if (userEntity == null)
            {
                return null;
            }

            return new UserInfo
            {
                Id = userEntity.Id,
                EmailAddress = userEntity.EmailAddress,
                Firstname = userEntity.Firstname,
                Lastname = userEntity.Lastname,
                Roles = RoleHelper.GetRoles(userEntity.RolesData)
            };
        }
    }
}
