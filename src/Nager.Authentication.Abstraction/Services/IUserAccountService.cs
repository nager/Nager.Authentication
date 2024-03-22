using Nager.Authentication.Abstraction.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
{
    /// <summary>
    /// User Account Service Interface
    /// </summary>
    public interface IUserAccountService
    {
        Task<bool> ChangePasswordAsync(
            string emailAddress,
            UserUpdatePasswordRequest userChangePasswordRequest,
            CancellationToken cancellationToken = default);
    }
}
