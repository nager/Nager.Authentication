using Nager.Authentication.Abstraction.Entities;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.Abstraction.Validators
{
    public interface IUserRepository
    {
        Task<UserEntity[]> QueryAsync(
            int take,
            int skip,
            Expression<Func<UserEntity, bool>>? predicate = default,
            CancellationToken cancellationToken = default);

        Task<UserEntity?> GetAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> AddAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> SetLastValidationTimestampAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<bool> SetLastSuccessfulValidationTimestampAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default);
    }
}
