using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class RemoveTodoItemTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Remove_Todo_Item_By_Id()
        {
            //Given
            var item = new TodoItem { Name = "An Item" };
            await Database.OnceAndSaveAsync(async db =>
            {
                var collection = new Collection { Name = "A Collection" };
                collection.TodoItems.Add(item);
                await db.Collections.AddAsync(collection);
            });

            //When
            var response = await Client.DeleteAsync($"/todo/{item.Id}");

            //Then
            response.EnsureSuccessStatusCode();
            var isExists = await Database.OnceAsync(async db =>
                await db.TodoItems.AnyAsync(i => i.Id.Equals(item.Id)));
            Assert.False(isExists);
        }
    }
}
