using Nager.Authentication.Abstraction.Entities;
using Nager.Authentication.Abstraction.Helpers;
using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Validators;
using System;
using System.Linq;
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
            var items = await this._userRepository.QueryAsync(take, skip, cancellationToken: cancellationToken);
            return items.OrderBy(user => user.EmailAddress).Select(userEntity => new UserInfo
            {
                Id = userEntity.Id,
                EmailAddress = userEntity.EmailAddress,
                Firstname = userEntity.Firstname,
                Lastname = userEntity.Lastname,
                Roles = RoleHelper.GetRoles(userEntity.RolesData)
            }).ToArray();
        }

        public async Task<UserInfo?> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.Id == id, cancellationToken);
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

        public async Task<UserInfo?> GetByEmailAddressAsync(
            string emailAddress,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.EmailAddress == emailAddress, cancellationToken);
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

        public async Task<string> ResetPasswordAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var userEntity = await this._userRepository.GetAsync(o => o.Id == id, cancellationToken);
            if (userEntity == null)
            {
                return null;
            }

            var randomPassword = PasswordHelper.CreateRandomPassword(10);

            var passwordSalt = PasswordHelper.CreateSalt();
            var passwordHash = PasswordHelper.HashPasword(randomPassword, passwordSalt);

            userEntity.PasswordSalt = passwordSalt;
            userEntity.PasswordHash = passwordHash;

            if (await this._userRepository.UpdateAsync(userEntity, cancellationToken))
            {
                return randomPassword;
            }

            return null;
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

            var userId = Guid.NewGuid().ToString();

            var passwordSalt = PasswordHelper.CreateSalt();
            var passwordHash = PasswordHelper.HashPasword(createUserRequest.Password, passwordSalt);

            var userEntity = new UserEntity
            {
                Id = userId,
                EmailAddress = createUserRequest.EmailAddress,
                Firstname = createUserRequest.Firstname,
                Lastname = createUserRequest.Lastname,
                RolesData = RoleHelper.GetRolesData(createUserRequest.Roles),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
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

            userEntity.RolesData = RoleHelper.AddRoleToRoleData(userEntity.RolesData, roleName);

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

            userEntity.RolesData = RoleHelper.RemoveRoleFromRoleData(userEntity.RolesData, roleName);

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
