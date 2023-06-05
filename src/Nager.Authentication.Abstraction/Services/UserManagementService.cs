using Nager.Authentication.Abstraction.Models;
using Nager.Authentication.Abstraction.Validators;
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
            return await this._userRepository.QueryAsync(take, skip, cancellationToken);
        }

        public async Task<UserInfo?> GetAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.GetAsync(id, cancellationToken);
        }

        public async Task<bool> ChangePasswordAsync(
            string id,
            UserChangePasswordRequest userChangePasswordRequest,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.ChangePasswordAsync(id, userChangePasswordRequest, cancellationToken);
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
            UserUpdateRequest updateUserRequest,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.UpdateAsync(id, updateUserRequest, cancellationToken);
        }

        public async Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            return await this._userRepository.DeleteAsync(id, cancellationToken);
        }
    }
}
