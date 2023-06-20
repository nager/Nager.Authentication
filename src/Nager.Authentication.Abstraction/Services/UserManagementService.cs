using Nager.Authentication.Abstraction.Entities;
using Nager.Authentication.Abstraction.Helpers;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Validators;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

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
            var items = await this._userRepository.QueryAsync(take, skip, cancellationToken: cancellationToken);
            return items.OrderBy(user => user.EmailAddress).Select(userEntity => new UserInfo
            {
                Id = userEntity.Id,
                EmailAddress = userEntity.EmailAddress,
                Firstname = userEntity.Firstname,
                Lastname = userEntity.Lastname,
                Roles = userEntity.RolesData.Split(',')
            }).ToArray();
        }

        public async Task<UserInfo?> GetAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.Id == id, cancellationToken);

            return new UserInfo
            {
                Id = userEntity.Id,
                EmailAddress = userEntity.EmailAddress,
                Firstname = userEntity.Firstname,
                Lastname = userEntity.Lastname,
                Roles = userEntity.RolesData.Split(',')
            };
        }

        public async Task<string> ResetPasswordAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var password = this.GenerateToken(10);
            var passwordHash = PasswordHelper.HashPasword(password, new byte[16]);

            //if (await this._userRepository.UpdatePasswordAsync(id, passwordHash, cancellationToken))
            //{
            //    return password;
            //}

            return null;
        }

        private string GenerateToken(int length)
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var tokenBuffer = new byte[length];
            rngCryptoServiceProvider.GetBytes(tokenBuffer);
            return Convert.ToBase64String(tokenBuffer);
        }

        public async Task<bool> CreateAsync(
            UserCreateRequest createUserRequest,
            CancellationToken cancellationToken = default)
        {
            var tempUserEntity = await this._userRepository.GetAsync(o => o.EmailAddress == createUserRequest.EmailAddress, cancellationToken);
            if (tempUserEntity != null)
            {
                return false;
            }

            var passwordHash = PasswordHelper.HashPasword(createUserRequest.Password, new byte[16]);

            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                EmailAddress = createUserRequest.EmailAddress,
                Firstname = createUserRequest.Firstname,
                Lastname = createUserRequest.Lastname,
                RolesData = string.Join(',', createUserRequest.Roles),
                PasswordHash = passwordHash
            };

            return await this._userRepository.AddAsync(userEntity, cancellationToken);
        }

        public async Task<bool> UpdateAsync(
            string id,
            UserUpdateNameRequest updateUserNameRequest,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.Id == id, cancellationToken);
            if (userEntity == null)
            {
                return false;
            }

            userEntity.Firstname = updateUserNameRequest.Firstname;
            userEntity.Lastname = updateUserNameRequest.Lastname;

            return await this._userRepository.UpdateAsync(userEntity);
        }

        public async Task<bool> AddRoleAsync(
            string id,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.Id == id, cancellationToken);
            if (userEntity == null)
            {
                return false;
            }

            
            var roles = JsonSerializer.Deserialize<string[]>(userEntity.RolesData);

            var roles2 = roles.ToList();
            roles2.Add(roleName);


            userEntity.RolesData = JsonSerializer.Serialize(roles2);

            //var roles = userEntity.RolesData
            //roles.Add(roleName);

            //userEntity.Roles = roles.ToArray();

            return await this._userRepository.UpdateAsync(userEntity);
        }

        public async Task<bool> RemoveRoleAsync(
            string id,
            string roleName,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.Id == id, cancellationToken);
            if (userEntity == null)
            {
                return false;
            }

            userEntity.RolesData = userEntity.RolesData;//.Where(o => o != roleName).ToArray();

            return await this._userRepository.UpdateAsync(userEntity);
        }

        public async Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.DeleteAsync(o => o.Id == id, cancellationToken);
        }
    }
}
