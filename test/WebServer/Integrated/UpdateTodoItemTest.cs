using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class UpdateTodoItemTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Update_Todo_Item_Name()
        {
            //Given
            const string name = "An Item";
            var item = await Database.OnceAndSaveAsync(async db =>
            {
                var item = new TodoItem { Name = name };
                var collection = new Collection { Name = "A Collection " };
                collection.TodoItems.Add(item);
                await db.AddAsync(collection);
                return item;
            });

            //When
            const string newName = "New Item Name";
            var response = await Scoped(async () =>
            {
                var input = new { name = newName };
                var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
                return await Client.PutAsync($"/todo/{item.Id}", content);
            });

            //Then
            response.EnsureSuccessStatusCode();
            var result = await Database.OnceAsync(async db =>
                await db.TodoItems.FindAsync(item.Id));
            Assert.Equal(newName, result.Name);
        }

        [Fact]
        public async Task Can_Update_Todo_Item_Description()
        {
            //Given
            const string description = "An Item Description";
            var item = await Database.OnceAndSaveAsync(async db =>
            {
                var item = new TodoItem { Name = "An Item", Description = description };
                var collection = new Collection { Name = "A Collection " };
                collection.TodoItems.Add(item);
                await db.AddAsync(collection);
                return item;
            });

            //When
            const string newDescription = "New Item Description";
            var response = await Scoped(async () =>
            {
                var input = new { description = newDescription };
                var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
                return await Client.PutAsync($"/todo/{item.Id}", content);
            });

            //Then
            response.EnsureSuccessStatusCode();
            var result = await Database.OnceAsync(async db =>
                await db.TodoItems.FindAsync(item.Id));
            Assert.Equal(newDescription, result.Description);
        }
    }
}
