using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class CreateTodoItemController
    {
        private readonly CollectionRepository _collectionRepository;
        private readonly TodoItemRepository _todoItemRepository;

        public CreateTodoItemController(CollectionRepository collectionRepository,
            TodoItemRepository todoItemRepository)
        {
            _collectionRepository = collectionRepository;
            _todoItemRepository = todoItemRepository;
        }

        [HttpPost("/collection/{id}/todo")]
        public async Task<IActionResult> CreateAsync(int id, InputModel input)
        {
            var collection = await _collectionRepository.FindAsync(id);
            var result = await _todoItemRepository.AddAsync(collection, input.Build());
            return new CreatedAtRouteResult("ShowItem", new { id = result.Id }, result);
        }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }

            internal TodoItem Build() => new TodoItem
            {
                Name = Name,
                Description = Description,
            };
        }
    }
}
