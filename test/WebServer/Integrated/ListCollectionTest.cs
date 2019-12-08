using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class ListCollectionTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_List_Collections()
        {
            // Given
            var collectionsNames = new[] { "Collection1", "Collection2", "Collection2" };
            await Database.OnceAndSaveAsync(async db =>
            {
                var collections = collectionsNames.Select(name => new Collection { Name = name });
                await db.AddRangeAsync(collections);
            });

            // When
            var response = await Client.GetAsync("/collection");

            // Then
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<Collection[]>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            Assert.Equal(collectionsNames.Length, result.Length);
            Assert.True(result.All(r => collectionsNames.Contains(r.Name)));
        }

        [Fact]
        public async Task Can_Limit_List_With_Request_Query()
        {
            // Given
            const int limit = 3;
            await Database.OnceAndSaveAsync(async db =>
            {
                var collections = Enumerable.Range(0, 10).Select(i => new Collection { Name = $"Collection{i}" });
                await db.AddRangeAsync(collections);
            });

            // When
            var response = await Client.GetAsync($"/collection?limit={limit}");

            // Then
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<Collection[]>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            Assert.Equal(limit, result.Length);
        }
    }
}
