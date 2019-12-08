using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class RemoveTodoItemController
    {
        private readonly TodoItemRepository _repository;

        public RemoveTodoItemController(TodoItemRepository repository)
        {
            _repository = repository;
        }

        [HttpDelete("/todo/{id}")]
        public async Task<IActionResult> RemoveAsync(int id)
        {
            var item = await _repository.FindAsync(id);
            await _repository.RemoveAsync(item);
            return new OkResult();
        }
    }
}
