using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ChangeTodoItemStateController
    {
        private readonly TodoItemRepository _repository;

        public ChangeTodoItemStateController(TodoItemRepository repository)
        {
            _repository = repository;
        }

        [HttpPatch("/todo/{id}/done")]
        public async Task<IActionResult> DoneAsync(int id)
        {
            var item = await _repository.FindAsync(id);
            await _repository.DoneAsync(item);
            return new OkResult();
        }

        [HttpPatch("/todo/{id}/undone")]
        public async Task<IActionResult> UndoneAsync(int id)
        {
            var item = await _repository.FindAsync(id);
            await _repository.UndoneAsync(item);
            return new OkResult();
        }
    }
}
