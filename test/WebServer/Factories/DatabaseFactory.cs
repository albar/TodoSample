using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoList.WebServer.Test.Factories
{
    public class DatabaseFactory
    {
        private readonly DbConnection _connection;

        public DatabaseFactory(DbConnection connection)
        {
            _connection = connection;
        }

        public TodoDbContext Create()
        {
            var options = new DbContextOptionsBuilder()
                .UseSqlite(_connection)
                .Options;

            var db = new TodoDbContext(options);
            db.Database.EnsureCreated();
            return db;
        }

        public async Task OnceAsync(Func<TodoDbContext, Task> action)
        {
            await using var db = Create();
            await action.Invoke(db);
        }

        public async Task<TResult> OnceAsync<TResult>(Func<TodoDbContext, Task<TResult>> action)
        {
            await using var db = Create();
            return await action.Invoke(db);
        }

        public async Task OnceAndSaveAsync(Func<TodoDbContext, Task> action)
        {
            await using var db = Create();
            await action.Invoke(db);
            await db.SaveChangesAsync();
        }

        public async Task<TResult> OnceAndSaveAsync<TResult>(Func<TodoDbContext, Task<TResult>> action)
        {
            await using var db = Create();
            var result = await action.Invoke(db);
            await db.SaveChangesAsync();
            return result;
        }
    }
}
