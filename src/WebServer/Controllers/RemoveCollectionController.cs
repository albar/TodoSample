using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class RemoveCollectionController
    {
        private readonly CollectionRepository _repository;

        public RemoveCollectionController(CollectionRepository repository)
        {
            _repository = repository;
        }

        [HttpDelete("/collection/{id}")]
        public async Task<IActionResult> RemoveAsync(int id)
        {
            var collection = await _repository.FindAsync(id);
            await _repository.RemoveAsync(collection);
            return new OkResult();
        }
    }
}
