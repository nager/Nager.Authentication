using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nager.Authentication.Abstraction.Entities;
using Nager.Authentication.Abstraction.Validators;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Nager.Authentication.InMemoryRepository
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ILogger<InMemoryUserRepository> _logger;

        private readonly ConcurrentDictionary<string, UserEntity> _userInfos;

        public InMemoryUserRepository(
            ILogger<InMemoryUserRepository>? logger = default)
        {
            this._logger = logger ?? new NullLogger<InMemoryUserRepository>();

            this._userInfos = new ConcurrentDictionary<string, UserEntity>();
        }

        public Task<UserEntity[]> QueryAsync(
            int take,
            int skip,
            Expression<Func<UserEntity, bool>>? predicate = default,
            CancellationToken cancellationToken = default)
        {
            var qurey = this._userInfos.Values.AsQueryable();
            if (predicate != default)
            {
                qurey = qurey.Where(predicate);
            }
            var items = qurey.Take(take).Skip(skip).ToArray();

            return Task.FromResult(items);
        }

        public Task<UserEntity> GetAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var item = this._userInfos.Values.AsQueryable().Where(predicate).FirstOrDefault();
            return Task.FromResult(item);
        }

        public Task<bool> AddAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default)
        {
            var isSuccessful = this._userInfos.TryAdd(entity.Id, entity);
            return Task.FromResult(isSuccessful);
        }

        public Task<bool> UpdateAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default)
        {
            var item = this._userInfos.Values.AsQueryable().Where(o => o.Id == entity.Id).FirstOrDefault();
            if (item == null)
            {
                return Task.FromResult(false);
            }

            item.EmailAddress = entity.EmailAddress;
            item.Firstname = entity.Firstname;
            item.Lastname = entity.Lastname;
            item.PasswordHash = entity.PasswordHash;
            item.PasswordSalt = entity.PasswordSalt;

            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var item = this._userInfos.Values.AsQueryable().Where(predicate).FirstOrDefault();
            
            var isSuccessful = this._userInfos.TryRemove(item.Id, out _);
            return Task.FromResult(isSuccessful);
        }
    }
}
