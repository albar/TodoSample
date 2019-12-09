using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories.Options;

namespace TodoList.WebServer.Repositories
{
    public class CollectionRepository
    {
        private TodoDbContext _database;

        public CollectionRepository(TodoDbContext database)
        {
            _database = database;
        }

        public async Task<Collection> AddAsync(Collection collection)
        {
            var value = _database.Add(collection);
            await _database.SaveChangesAsync();
            value.State = EntityState.Detached;
            return value.Entity;
        }

        public async Task<Collection> FindAsync(int id)
        {
            return await _database.Collections
                .AsNoTracking()
                .SingleAsync(c => c.Id == id);
        }

        public async Task<TResult> FindAsync<TResult>(int id, Expression<Func<Collection, TResult>> predicate)
        {
            return await _database.Collections
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(predicate)
                .FirstAsync();
        }

        public async Task<Collection[]> ListAsync()
        {
            return await _database.Collections.AsNoTracking().ToArrayAsync();
        }

        public async Task<Collection[]> ListAsync(IQueryOptions<Collection> options)
        {
            var query = options.BuildIntoQuery(_database.Collections.AsQueryable());
            return await query.AsNoTracking().ToArrayAsync();
        }

        public async Task<bool> UpdateAsync(Collection collection)
        {
            var entry = _database.Update(collection);
            await _database.SaveChangesAsync();
            var isSucceed = entry.State == EntityState.Unchanged;
            entry.State = EntityState.Detached;
            return isSucceed;
        }

        public async Task<bool> RemoveAsync(Collection collection)
        {
            var entry = _database.Remove(collection);
            await _database.SaveChangesAsync();
            return entry.State == EntityState.Detached;
        }
    }
}
