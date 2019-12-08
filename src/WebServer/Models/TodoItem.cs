namespace TodoList.WebServer.Models
{
    public class TodoItem
    {
        public int Id { get; internal set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public Collection Collection { get; set; }
    }
}
