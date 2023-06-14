using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Validators;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.InMemoryRepository
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ILogger<InMemoryUserRepository> _logger;

        private readonly ConcurrentDictionary<string, UserInfoWithPassword> _userInfos;

        public InMemoryUserRepository(
            UserInfoWithPassword[] userInfos,
            ILogger<InMemoryUserRepository>? logger = default)
        {
            this._logger = logger ?? new NullLogger<InMemoryUserRepository>();

            this._userInfos = new ConcurrentDictionary<string, UserInfoWithPassword>();

            foreach (var userInfo in userInfos)
            {
                var id = Guid.NewGuid().ToString();

                userInfo.Id = id;

                if (!this._userInfos.TryAdd(id, userInfo))
                {
                    this._logger.LogError($"{nameof(InMemoryUserRepository)} - Cannot add user {userInfo.EmailAddress}");
                }
            }
        }

        public Task<UserInfo> GetUserInfoAsync(string emailAddress)
        {
            var userInfoWithPassword = this._userInfos.Where(userInfo => userInfo.Value.EmailAddress == emailAddress.ToLower())
                .Select(o => o.Value)
                .SingleOrDefault();

            if (userInfoWithPassword == null)
            {
                return Task.FromResult<UserInfo>(null);
            }

            var userInfo = new UserInfo
            {
                Id = userInfoWithPassword.Id,
                EmailAddress = userInfoWithPassword.EmailAddress,
                Roles = userInfoWithPassword.Roles,
                Firstname = userInfoWithPassword.Firstname,
                Lastname = userInfoWithPassword.Lastname
            };

            return Task.FromResult(userInfo);
        }

        public Task<UserInfo[]> QueryAsync(
            int take,
            int skip,
            CancellationToken cancellationToken = default)
        {
            var userInfos = this._userInfos.Select(userInfo => new UserInfo
            {
                Id = userInfo.Key,
                EmailAddress = userInfo.Value.EmailAddress,
                Firstname = userInfo.Value.Firstname,
                Lastname = userInfo.Value.Lastname,
                Roles = userInfo.Value.Roles
            })
            .Skip(skip)
            .Take(take)
            .ToArray();

            return Task.FromResult(userInfos);
        }

        public Task<UserInfo?> GetAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            if (!this._userInfos.TryGetValue(id, out var value))
            {
                return Task.FromResult<UserInfo?>(null);
            }

            var userInfo = new UserInfo
            {
                Id = id,
                EmailAddress = value.EmailAddress,
                Roles = value.Roles,
                Firstname = value.Firstname,
                Lastname = value.Lastname,
            };

            return Task.FromResult<UserInfo?>(userInfo);
        }

        public Task<UserInfo?> ValidateAsync(
            string emailAddress,
            string passwordHash,
            CancellationToken cancellationToken = default)
        {
            var userInfo = this._userInfos
                .Where(userInfo => userInfo.Value.EmailAddress == emailAddress && userInfo.Value.Password == passwordHash)
                .Select(o => new UserInfo
                {
                    Id = o.Key,
                    EmailAddress = o.Value.EmailAddress,
                    Firstname = o.Value.Firstname,
                    Lastname = o.Value.Lastname,
                    Roles = o.Value.Roles
                }).SingleOrDefault();

            return Task.FromResult<UserInfo?>(userInfo);
        }

        public Task<bool> ExistsAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var exists = this._userInfos.Where(o=>o.Value.EmailAddress == emailAddress).Any();
            return Task.FromResult(exists);
        }

        public Task<bool> CreateAsync(
            UserCreateRequest createUserRequest,
            CancellationToken cancellationToken = default)
        {
            var id = Guid.NewGuid().ToString();

            var userInfoWithPassword = new UserInfoWithPassword
            {
                Id = id,
                EmailAddress = createUserRequest.EmailAddress,
                Password = createUserRequest.Password,
                Firstname = createUserRequest.Firstname,
                Lastname = createUserRequest.Lastname
            };

            if (this._userInfos.TryAdd(id, userInfoWithPassword))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> UpdateNameAsync(
            string id,
            UserUpdateNameRequest userUpdateNameRequest,
            CancellationToken cancellationToken = default)
        {
            if (this._userInfos.TryGetValue(id, out var userInfo))
            {
                userInfo.Firstname = userUpdateNameRequest.Firstname;
                userInfo.Lastname = userUpdateNameRequest.Lastname;

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> UpdatePasswordAsync(
            string id,
            UserUpdatePasswordRequest userUpdatePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            if (this._userInfos.TryGetValue(id, out var userInfo))
            {
                userInfo.Password = userUpdatePasswordRequest.Password;

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> AddRoleAsync(
            string id,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            if (this._userInfos.TryGetValue(id, out var userInfo))
            {
                var roles = userInfo.Roles.ToList();
                roles.Add(roleName);
                userInfo.Roles = roles.ToArray();

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> RemoveRoleAsync(
            string id,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            if (this._userInfos.TryGetValue(id, out var userInfo))
            {
                var roles = userInfo.Roles.ToList();
                roles.Remove(roleName);
                userInfo.Roles = roles.ToArray();

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            if (this._userInfos.TryRemove(id, out _))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
