using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Repositories;
using TodoList.WebServer.Repositories.Options;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    public class ListCollectionController
    {
        private readonly CollectionRepository _repository;

        public ListCollectionController(CollectionRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet("/collection")]
        public async Task<IActionResult> ListCollectionAsync([FromQuery] CollectionListOptions options)
        {
            var collections = await _repository.ListAsync(options);
            return new JsonResult(collections);
        }
    }
}
