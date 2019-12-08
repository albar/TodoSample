using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class CreateTodoItemTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Add_Todo_Item_To_A_Collection()
        {
            //Given
            var collection = new Collection { Name = "A Collection " };
            await Database.OnceAndSaveAsync(async db => await db.Collections.AddAsync(collection));
            var input = new
            {
                Name = "A Todo Item",
                Description = "A Todo Item Description"
            };

            //When
            var response = await Scoped(async () =>
            {
                var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
                return await Client.PostAsync($"/collection/{collection.Id}/todo", content);
            });

            //Then
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<TodoItem>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            Assert.Equal(input.Name, result.Name);
            Assert.Equal(input.Description, result.Description);
            Assert.False(result.IsDone);
            var isExists = await Database.OnceAsync(async db =>
                await db.TodoItems.AnyAsync(item =>
                    item.Collection.Id.Equals(collection.Id) && item.Name.Equals(input.Name)));
            Assert.True(isExists);
        }

        [Fact]
        public async Task Should_Specify_Name()
        {
            //Given
            var collection = new Collection { Name = "A Collection " };
            await Database.OnceAndSaveAsync(async db => await db.Collections.AddAsync(collection));
            var input = new { };

            //When
            var response = await Scoped(async () =>
            {
                var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
                return await Client.PostAsync($"/collection/{collection.Id}/todo", content);
            });

            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
