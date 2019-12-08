using System.Collections.Generic;

namespace TodoList.WebServer.Models
{
    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<TodoItem> TodoItems { get; } = new List<TodoItem>();
    }
}
