using System.Linq;
using TodoList.WebServer.Models;

namespace TodoList.WebServer.Repositories.Options
{
    public class CollectionListOptions : IQueryOptions<Collection>
    {
        public int? Limit { get; set; }

        public IQueryable<Collection> BuildIntoQuery(IQueryable<Collection> queryable)
        {


            // Limit should be last
            if (Limit is int limit)
            {
                queryable = queryable.Take(limit);
            }

            return queryable;
        }
    }
}
