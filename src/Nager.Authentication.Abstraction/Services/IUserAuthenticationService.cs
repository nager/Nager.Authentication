using Nager.Authentication.Abstraction.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Services
{
    public interface IUserAuthenticationService
    {
        Task<AuthenticationStatus> ValidateCredentialsAsync(
            AuthenticationRequest authenticationRequest,
            CancellationToken cancellationToken = default);

        Task<string[]> GetRolesAsync(
            string emailAddress,
            CancellationToken cancellationToken = default);
    }
}
