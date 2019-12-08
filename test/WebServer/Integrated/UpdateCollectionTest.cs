using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class UpdateCollectionTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Change_Collection_Name()
        {
            //Given
            const string name = "A Collection";
            var collection = await Database.OnceAndSaveAsync(async db =>
            {
                var collection = new Collection { Name = name };
                await db.AddAsync(collection);
                return collection;
            });

            //When
            const string newName = "New Collection Name";
            var response = await Scoped(async () =>
            {
                var input = new { name = newName };
                var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
                return await Client.PutAsync($"/collection/{collection.Id}", content);
            });

            //Then
            response.EnsureSuccessStatusCode();
            var result = await Database.OnceAsync(async db =>
                await db.Collections.FindAsync(collection.Id));
            Assert.Equal(newName, result.Name);
        }
    }
}
