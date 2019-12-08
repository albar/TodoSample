using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class CreateCollectionController
    {
        private readonly CollectionRepository _repository;

        public CreateCollectionController(CollectionRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("/collection")]
        public async Task<IActionResult> CreateAsync(InputModel input)
        {
            var result = await _repository.AddAsync(input.Build());
            return new CreatedAtRouteResult(
                "ShowCollection",
                new { id = result.Id },
                result);
        }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }

            internal Collection Build() => new Collection
            {
                Name = Name,
            };
        }
    }
}
