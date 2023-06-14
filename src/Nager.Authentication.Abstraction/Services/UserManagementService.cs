using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Validators;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;

        public UserManagementService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        public async Task<UserInfo[]> QueryAsync(
            int take,
            int skip,
            CancellationToken cancellationToken = default)
        {
            var items = await this._userRepository.QueryAsync(take, skip, cancellationToken);
            return items.OrderBy(user => user.EmailAddress).ToArray();
        }

        public async Task<UserInfo?> GetAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.GetAsync(id, cancellationToken);
        }

        public async Task<string> ResetPasswordAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var userChangePasswordRequest = new UserUpdatePasswordRequest
            {
                Password = this.GenerateToken(10)
            };

            if (await this._userRepository.UpdatePasswordAsync(id, userChangePasswordRequest, cancellationToken))
            {
                return userChangePasswordRequest.Password;
            }

            return null;
        }

        private string GenerateToken(int length)
        {
            using (RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var tokenBuffer = new byte[length];
                rngCryptoServiceProvider.GetBytes(tokenBuffer);
                return Convert.ToBase64String(tokenBuffer);
            }
        }

        public async Task<bool> CreateAsync(
            UserCreateRequest createUserRequest,
            CancellationToken cancellationToken = default)
        {
            if (await this._userRepository.ExistsAsync(createUserRequest.EmailAddress, cancellationToken))
            {
                return false;
            }

            return await this._userRepository.CreateAsync(createUserRequest, cancellationToken);
        }

        public async Task<bool> UpdateAsync(
            string id,
            UserUpdateNameRequest updateUserNameRequest,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.UpdateNameAsync(id, updateUserNameRequest, cancellationToken);
        }

        public async Task<bool> AddRoleAsync(
            string id,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.AddRoleAsync(id, roleName, cancellationToken);
        }

        public async Task<bool> RemoveRoleAsync(
            string id,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.RemoveRoleAsync(id, roleName, cancellationToken);
        }

        public async Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
