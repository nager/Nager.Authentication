using Nager.Authentication.Abstraction.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Validators
{
    public interface IUserRepository
    {
        Task<UserInfo> GetUserInfoAsync(string emailAddress);

        Task<UserInfo[]> QueryAsync(
            int take,
            int skip,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            string emailAddress,
            CancellationToken cancellationToken = default);

        Task<UserInfo?> GetAsync(
            string id,
            CancellationToken cancellationToken = default);

        Task<UserInfo?> ValidateAsync(
            string emailAddress,
            string passwordHash,
            CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(
            UserCreateRequest userCreateRequest,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateNameAsync(
            string id,
            UserUpdateNameRequest userUpdateNameRequest,
            CancellationToken cancellationToken = default);

        Task<bool> UpdatePasswordAsync(
            string id,
            UserUpdatePasswordRequest userUpdatePasswordRequest,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default);
    }
}
