using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class ShowCollectionTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Show_A_Collection()
        {
            //Given
            var toBeShowed = await Database.OnceAndSaveAsync(async db =>
            {
                var collections = new[]
                {
                    new Collection { Name = "Colection1" },
                    new Collection { Name = "Colection2" },
                };
                await db.Collections.AddRangeAsync(collections);
                return collections.First();
            });

            //When
            var response = await Client.GetAsync($"/collection/{toBeShowed.Id}");

            //Then
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<Collection>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            Assert.Equal(toBeShowed.Name, result.Name);
        }
    }
}
