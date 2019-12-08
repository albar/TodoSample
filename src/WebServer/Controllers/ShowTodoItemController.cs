using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ShowTodoItemController
    {
        private readonly TodoItemRepository _repository;

        public ShowTodoItemController(TodoItemRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("/todo/{id}", Name = "ShowItem")]
        public async Task<TodoItem> ShowAsync(int id)
        {
            return await _repository.FindAsync(id);
        }
    }
}
