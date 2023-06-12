using Nager.Authentication.Abstraction.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
{
    public interface IUserManagementService
    {
        Task<UserInfo[]> QueryAsync(
            int take,
            int skip,
            CancellationToken cancellationToken = default);

        Task<UserInfo?> GetAsync(
            string id,
            CancellationToken cancellationToken = default);

        Task<bool> CreateAsync(
            UserCreateRequest createUserRequest,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            string id,
            UserUpdateNameRequest updateUserRequest,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default);
    }
}
