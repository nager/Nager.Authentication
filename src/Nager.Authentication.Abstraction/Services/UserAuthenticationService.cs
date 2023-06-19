﻿using Microsoft.Extensions.Caching.Memory;
using Nager.Authentication.Abstraction.Helpers;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Validators;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
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
                throw new ArgumentNullException(nameof(authenticationInfo));
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
            if (await this.IsIpAddressBlockedAsync(authenticationRequest.IpAddress))
            {
                return AuthenticationStatus.TemporaryBlocked;
            }

            var authenticationCredentials = new AuthenticationCredentials
            {
                EmailAddress = authenticationRequest.EmailAddress,
                Password = authenticationRequest.Password
            };

            var passwordHash = PasswordHelper.HashPasword(authenticationRequest.Password, new byte[16]);

            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == authenticationRequest.EmailAddress && o.PasswordHash.SequenceEqual(passwordHash), cancellationToken);
            if (userEntity == null)
            {
                this.SetInvalidLogin(authenticationRequest.IpAddress);
                return AuthenticationStatus.Invalid;
            }

            this.SetValidLogin(authenticationRequest.IpAddress);
            return AuthenticationStatus.Valid;
        }

        public async Task<UserInfo> GetUserInfoAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == emailAddress);
            return new UserInfo
            {
                Id = userEntity.Id,
                EmailAddress = userEntity.EmailAddress,
                Firstname = userEntity.Firstname,
                Lastname = userEntity.Lastname,
                Roles = userEntity.Roles
            };
        }
    }
}
