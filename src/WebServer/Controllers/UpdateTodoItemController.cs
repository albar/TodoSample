using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class UpdateTodoItemController
    {
        private readonly TodoItemRepository _repository;

        public UpdateTodoItemController(TodoItemRepository repository)
        {
            _repository = repository;
        }

        [HttpPut("/todo/{id}")]
        public async Task<IActionResult> UpdateAsync(int id, InputModel input)
        {
            var item = await _repository.FindAsync(id);
            input.PutInto(item);
            await _repository.UpdateAsync(item);
            return new OkResult();
        }

        public class InputModel
        {
            public string Name { get; set; }
            public string Description { get; set; }

            internal void PutInto(TodoItem item)
            {
                item.Name = Name ?? item.Name;
                item.Description = Description ?? item.Description;
            }
        }
    }
}
