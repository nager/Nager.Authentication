using Microsoft.EntityFrameworkCore;
using Nager.Authentication.Abstraction.Entities;
using Nager.Authentication.Abstraction.Validators;
using System.Linq.Expressions;

namespace Nager.Authentication.MssqlRepository
{
    public class MssqlUserRepository : IUserRepository
    {
        private readonly DatabaseContext _databaseContext;

        public MssqlUserRepository(DatabaseContext databaseContext)
        {
            this._databaseContext = databaseContext;
        }

        public async Task<UserEntity[]> QueryAsync(
            int take,
            int skip,
            Expression<Func<UserEntity, bool>>? predicate = default,
            CancellationToken cancellationToken = default)
        {
            var query = this._databaseContext.Users.AsQueryable();
            if (predicate != null )
            {
                query = query.Where(predicate);
            }

            return await query.Skip(skip).Take(take).ToArrayAsync();
        }

        public async Task<UserEntity?> GetAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var query = this._databaseContext.Users.AsQueryable();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> AddAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default)
        {
            await this._databaseContext.AddAsync(entity, cancellationToken);

            return true;
        }

        public async Task<bool> UpdateAsync(
            UserEntity entity,
            CancellationToken cancellationToken = default)
        {
            var existingItem = await this._databaseContext.Users.SingleOrDefaultAsync(o => o.Id == entity.Id, cancellationToken);
            existingItem.Firstname = entity.Firstname;
            existingItem.Lastname = entity.Lastname;
            existingItem.EmailAddress = entity.EmailAddress;
            existingItem.PasswordHash = entity.PasswordHash;
            existingItem.RolesData = entity.RolesData;

            await this._databaseContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> DeleteAsync(
            Expression<Func<UserEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            var query = this._databaseContext.Users.AsQueryable().Where(predicate);

            var items = await this._databaseContext.Users.Where(predicate).ToArrayAsync(cancellationToken);
            this._databaseContext.Users.RemoveRange(items);
            await this._databaseContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
