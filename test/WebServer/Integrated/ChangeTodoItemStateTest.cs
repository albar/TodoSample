using System.Threading.Tasks;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class ChangeTodoItemStateTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Mark_Todo_Item_As_Done()
        {
            //Given
            var item = await Database.OnceAndSaveAsync(async db =>
            {
                var collection = new Collection { Name = "A Collection" };
                var item = new TodoItem { Name = "An Item" };
                collection.TodoItems.Add(item);
                await db.Collections.AddAsync(collection);
                return item;
            });

            //When
            var response = await Client.PatchAsync($"/todo/{item.Id}/done", null);

            //Then
            response.EnsureSuccessStatusCode();
            var theItem = await Database.OnceAsync(async db =>
                await db.TodoItems.FindAsync(item.Id));
            Assert.True(theItem.IsDone);
        }

        [Fact]
        public async Task Can_Mark_Todo_Item_As_Undone()
        {
            //Given
            var item = await Database.OnceAndSaveAsync(async db =>
            {
                var collection = new Collection { Name = "A Collection" };
                var item = new TodoItem { Name = "An Item", IsDone = true };
                collection.TodoItems.Add(item);
                await db.Collections.AddAsync(collection);
                return item;
            });

            //When
            var response = await Client.PatchAsync($"/todo/{item.Id}/undone", null);

            //Then
            response.EnsureSuccessStatusCode();
            var theItem = await Database.OnceAsync(async db =>
                await db.TodoItems.FindAsync(item.Id));
            Assert.False(theItem.IsDone);
        }
    }
}
