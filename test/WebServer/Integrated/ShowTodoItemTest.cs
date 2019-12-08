using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class ShowTodoItemTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Show_A_Todo_Item()
        {
            //Given
            var collection = await Database.OnceAndSaveAsync(async db =>
            {
                var collection = new Collection { Name = "A Collection" };
                collection.TodoItems.Add(new TodoItem { Name = "Item1" });
                collection.TodoItems.Add(new TodoItem { Name = "Item2" });
                await db.Collections.AddAsync(collection);
                return collection;
            });
            var toBeShown = collection.TodoItems.First();

            //When
            var response = await Client.GetAsync($"/todo/{toBeShown.Id}");

            //Then
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<TodoItem>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            Assert.Equal(toBeShown.Name, result.Name);
            Assert.Equal(collection.Name, result.Collection.Name);
            Assert.Equal(collection.Id, result.Collection.Id);
        }
    }
}
