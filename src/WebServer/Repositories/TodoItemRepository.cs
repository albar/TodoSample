using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;

namespace TodoList.WebServer.Repositories
{
    public class TodoItemRepository
    {
        private TodoDbContext _database;

        public TodoItemRepository(TodoDbContext database)
        {
            _database = database;
        }

        public async Task<TodoItem> AddAsync(Collection collection, TodoItem item)
        {
            _database.Attach(collection);
            collection.TodoItems.Add(item);
            await _database.SaveChangesAsync();
            return item;
        }

        public async Task<TodoItem> FindAsync(int id)
        {
            return await _database.TodoItems
                .Include(item => item.Collection)
                .FirstAsync(item => item.Id.Equals(id));
        }

        public async Task<bool> DoneAsync(TodoItem item)
        {
            item.IsDone = true;
            return await UpdateAsync(item);
        }

        public async Task<bool> UndoneAsync(TodoItem item)
        {
            item.IsDone = false;
            return await UpdateAsync(item);
        }

        public async Task<bool> UpdateAsync(TodoItem item)
        {
            var entry = _database.Update(item);
            await _database.SaveChangesAsync();
            return entry.State == EntityState.Unchanged;
        }

        public async Task<bool> RemoveAsync(TodoItem item)
        {
            var entry = _database.Remove(item);
            await _database.SaveChangesAsync();
            return entry.State == EntityState.Detached;
        }
    }
}
