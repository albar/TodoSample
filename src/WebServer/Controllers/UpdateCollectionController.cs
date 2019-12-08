using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class UpdateCollectionController
    {
        private readonly CollectionRepository _repositoy;

        public UpdateCollectionController(CollectionRepository repositoy)
        {
            _repositoy = repositoy;
        }

        [HttpPut("/collection/{id}")]
        public async Task<IActionResult> UpdateAsync(int id, InputModel input)
        {
            var collection = await _repositoy.FindAsync(id);
            input.PutInto(collection);
            await _repositoy.UpdateAsync(collection);
            return new OkResult();
        }

        public class InputModel
        {
            public string Name { get; set; }

            internal void PutInto(Collection collection)
            {
                collection.Name = Name ?? collection.Name;
            }
        }
    }
}
