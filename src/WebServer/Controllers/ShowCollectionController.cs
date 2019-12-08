using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;

namespace TodoList.WebServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ShowCollectionController
    {
        private readonly CollectionRepository _repository;

        public ShowCollectionController(CollectionRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("/collection/{id}", Name = "ShowCollection")]
        public async Task<Collection> ShowAsync(int id)
        {
            return await _repository.FindAsync(id);
        }
    }
}
