using System;
using System.Linq;
using TodoList.WebServer.Models;

namespace TodoList.WebServer.Repositories.Options
{
    public interface IQueryOptions<T>
    {
        IQueryable<Collection> BuildIntoQuery(IQueryable<Collection> queryable);
    }
}
