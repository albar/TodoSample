using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class CreateCollectionTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Create_A_Collection()
        {
            //Given
            await Database.OnceAndSaveAsync(db => db.Database.EnsureCreatedAsync());
            var input = new { Name = "A Collection" };

            //When
            var response = await Scoped(async () =>
            {
                var content = new StringContent(JsonSerializer.Serialize(input), Encoding.UTF8, "application/json");
                return await Client.PostAsync("/collection", content);
            });

            //Then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var result = JsonSerializer.Deserialize<Collection>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            Assert.Equal(input.Name, result.Name);
        }

        [Fact]
        public async Task Should_Specify_A_Name()
        {
            //Given
            var collection = new { };

            //When
            var response = await Scoped(async () =>
            {
                var content = new StringContent(JsonSerializer.Serialize(collection), Encoding.UTF8, "application/json");
                return await Client.PostAsync("/collection", content);
            });

            //Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
