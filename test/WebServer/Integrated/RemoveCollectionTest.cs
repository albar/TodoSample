using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;
using Xunit;

namespace TodoList.WebServer.Test.Integrated
{
    public class RemoveCollectionTest : IntegratedTestBase
    {
        [Fact]
        public async Task Can_Remove_A_Collection()
        {
            //Given
            var toBeRemoved = await Database.OnceAndSaveAsync(async db =>
            {
                var collections = new[]
                {
                    new Collection { Name = "Collection1"},
                    new Collection { Name = "Collection2"},
                    new Collection { Name = "Collection3"},
                };
                await db.Collections.AddRangeAsync(collections);
                return collections.First();
            });

            //When
            var response = await Client.DeleteAsync($"/collection/{toBeRemoved.Id}");

            //Then
            response.EnsureSuccessStatusCode();
            var isExists = await Database.OnceAsync(async db =>
                await db.Collections.AnyAsync(c => c.Name.Equals(toBeRemoved.Name)));
            Assert.False(isExists);
        }
    }
}
