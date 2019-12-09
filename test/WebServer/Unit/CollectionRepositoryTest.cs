using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;
using TodoList.WebServer.Repositories.Options;
using Xunit;

namespace TodoList.WebServer.Test.Unit
{
    public class CollectionRepositoryTest : UnitTestBase
    {
        [Fact]
        public async Task Can_Add_A_New_Collection()
        {
            //Given
            var collection = new Collection { Name = "A Collection" };

            //When
            await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);
                await repository.AddAsync(collection);
            });

            //Then
            var isExists = await Database.OnceAsync(async db =>
                await db.Collections.AnyAsync(c => c.Name.Equals(collection.Name)));

            Assert.True(isExists);
        }

        [Fact]
        public async Task Added_Collection_Should_Not_Be_Tracked()
        {
            //Given
            const string realName = "A Collection";
            var collection = new Collection { Name = realName };

            //When
            await Database.OnceAsync(async db =>
            {
                var repo = new CollectionRepository(db);
                await repo.AddAsync(collection);

                collection.Name = "Manipulated Name";
                await db.SaveChangesAsync();
            });

            //Then
            var result = await Database.OnceAsync(async db =>
                await db.Collections.FindAsync(collection.Id));
            Assert.Equal(realName, result.Name);
        }

        [Fact]
        public async Task Can_Find_A_Collection_By_id()
        {
            //Given
            var toBeFound = await Database.OnceAndSaveAsync(async db =>
            {
                var collections = new[]
                {
                    new Collection{ Name = "Collection1" },
                    new Collection{ Name = "Collection2" },
                };
                await db.Collections.AddRangeAsync(collections);
                return collections.First();
            });

            //When
            var found = await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);
                return await repository.FindAsync(toBeFound.Id);
            });

            //Then
            Assert.Equal(toBeFound.Name, found.Name);
        }

        [Fact]
        public async Task Found_Collection_Should_Not_Be_Tracked()
        {
            //Given
            const string realName = "A Collection";
            var collectionId = await Database.OnceAsync(async db =>
            {
                var repo = new CollectionRepository(db);
                var collection = new Collection { Name = realName };
                await repo.AddAsync(collection);
                return collection.Id;
            });

            //When
            await Database.OnceAsync(async db =>
            {
                var repo = new CollectionRepository(db);
                var collection = await repo.FindAsync(collectionId);

                collection.Name = "Manipulated Name";
                await db.SaveChangesAsync();
            });

            //Then
            var result = await Database.OnceAsync(async db =>
                await db.Collections.FindAsync(collectionId));

            Assert.Equal(realName, result.Name);
        }

        [Fact]
        public async Task Can_Find_And_Map_Result()
        {
            //Given
            const int itemsCount = 3;
            var collection = await Database.OnceAndSaveAsync(async db =>
            {
                var collection = new Collection { Name = "A Collection" };
                for (var i = 0; i < itemsCount; i++)
                {
                    collection.TodoItems.Add(new TodoItem { Name = $"Item{i}" });
                }
                await db.Collections.AddAsync(collection);
                return collection;
            });

            //When
            var result = await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);

                return await repository.FindAndMapAsync(collection.Id, c => new
                {
                    Collection = c,
                    TodoItemsCount = c.TodoItems.Count,
                });
            });

            //Then
            Assert.Equal(collection.Id, result.Collection.Id);
            Assert.Equal(itemsCount, result.TodoItemsCount);
        }

        [Fact]
        public async Task Mapped_Found_Collection_Should_Not_Be_Tracked()
        {
            //Given
            const string realName = "A Collection";
            const int itemsCount = 3;
            var collectionId = await Database.OnceAsync(async db =>
            {
                var repo = new CollectionRepository(db);
                var collection = new Collection { Name = realName };
                for (var i = 0; i < itemsCount; i++)
                {
                    collection.TodoItems.Add(new TodoItem { Name = $"Item{i}" });
                }
                await repo.AddAsync(collection);
                return collection.Id;
            });

            //When
            await Database.OnceAsync(async db =>
            {
                var repo = new CollectionRepository(db);
                var found = await repo.FindAndMapAsync(collectionId, collection => new
                {
                    Collection = collection,
                    TodoItemsCount = collection.TodoItems.Count,
                });

                Assert.Equal(itemsCount, found.TodoItemsCount);

                found.Collection.Name = "Manipulated Name";
                await db.SaveChangesAsync();
            });

            //Then
            var result = await Database.OnceAsync(async db =>
                await db.Collections.FindAsync(collectionId));

            Assert.Equal(realName, result.Name);
        }

        [Fact]
        public async Task Can_List_Collections()
        {
            // Given
            var collectionsNames = new[] { "Collection1", "Collection3", "Collection2" };
            await Database.OnceAndSaveAsync(async db =>
            {
                var collections = collectionsNames.Select(name => new Collection { Name = name });
                await db.AddRangeAsync(collections);
            });

            // When
            var collections = await Database.OnceAsync<Collection[]>(async db =>
                await new CollectionRepository(db).ListAsync());

            // Then
            Assert.Equal(collectionsNames.Length, collections.Length);
            Assert.True(collections.All(collection => collectionsNames.Contains(collection.Name)));
        }

        [Fact]
        public async Task Can_Query_List_Collection()
        {
            // Given
            const int limit = 3;
            const int count = 10;
            await Database.OnceAndSaveAsync(async db =>
            {
                var collections = Enumerable.Range(0, count).Select(i => new Collection { Name = $"Collection{i}" });
                await db.AddRangeAsync(collections);
            });

            // When
            var collections = await Database.OnceAsync(async db =>
            {
                var options = new CollectionListOptions
                {
                    Limit = limit
                };

                return await new CollectionRepository(db).ListAsync(options);
            });

            // Then
            Assert.Equal(limit, collections.Length);
            var inDbCount = await Database.OnceAsync(async db => await db.Collections.CountAsync());
            Assert.Equal(count, inDbCount);
        }

        [Fact]
        public async Task Listed_Collections_Should_Not_Be_Tracked()
        {
            // Given
            var collectionsNames = new[] { "Collection1", "Collection2", "Collection3" };
            await Database.OnceAndSaveAsync(async db =>
            {
                var collections = collectionsNames.Select(name => new Collection { Name = name });
                await db.AddRangeAsync(collections);
            });

            // When
            await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);
                var collections = await repository.ListAsync();

                foreach (var collection in collections)
                {
                    collection.Name = $"Manipulated {collection.Name}";
                }

                await db.SaveChangesAsync();
            });

            // Then
            var collections = await Database.OnceAsync(async db =>
                await db.Collections.ToArrayAsync());

            Assert.True(collections.All(collection => collectionsNames.Contains(collection.Name)));
        }

        [Fact]
        public async Task Listed_Queryable_Collections_Should_Not_Be_Tracked()
        {
            // Given
            const string prefix = "Collection";
            await Database.OnceAndSaveAsync(async db =>
            {
                var collections = Enumerable.Range(0, 10).Select(i => new Collection { Name = $"{prefix}{i}" });
                await db.AddRangeAsync(collections);
            });

            // When
            await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);
                var options = new CollectionListOptions
                {
                    Limit = 3,
                };
                var collections = await repository.ListAsync(options);

                foreach (var collection in collections)
                {
                    collection.Name = $"Manipulated {collection.Name}";
                }

                await db.SaveChangesAsync();
            });

            // Then
            var collections = await Database.OnceAsync(async db =>
                await db.Collections.ToArrayAsync());
            Assert.True(collections.All(collection => collection.Name.StartsWith(prefix)));
        }

        [Fact]
        public async Task Can_Update_A_Collection()
        {
            //Given
            const string newName = "New Collection Name";
            var collection = new Collection { Name = "A Collection" };
            await Database.OnceAndSaveAsync(async db => await db.Collections.AddAsync(collection));

            //When
            var isSucceed = await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);

                collection.Name = newName;
                return await repository.UpdateAsync(collection);
            });

            //Then
            Assert.True(isSucceed);
            var updated = await Database.OnceAsync(async db =>
                await db.Collections.FindAsync(collection.Id));
            Assert.Equal(newName, updated.Name);
        }

        [Fact]
        public async Task Updated_Collection_Should_Not_Be_Tracked()
        {
            //Given
            const string newName = "New Collection Name";
            var collection = new Collection { Name = "A Collection" };
            await Database.OnceAndSaveAsync(async db => await db.Collections.AddAsync(collection));

            //When
            await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);

                collection.Name = newName;
                await repository.UpdateAsync(collection);

                collection.Name = "Manipulated Name";
                await db.SaveChangesAsync();
            });

            //Then
            var updated = await Database.OnceAsync(async db =>
                await db.Collections.FindAsync(collection.Id));
            Assert.Equal(newName, updated.Name);
        }

        [Fact]
        public async Task Can_Remove_A_Collection()
        {
            //Given
            var toBeRemoved = await Database.OnceAndSaveAsync(async db =>
            {
                var collections = new[]
                {
                    new Collection { Name = "Collection1" },
                    new Collection { Name = "Collection2" },
                    new Collection { Name = "Collection3" },
                };

                await db.Collections.AddRangeAsync(collections);

                return collections.First();
            });

            //When
            var isSucceed = await Database.OnceAsync(async db =>
            {
                var repository = new CollectionRepository(db);
                return await repository.RemoveAsync(toBeRemoved);
            });

            //Then
            Assert.True(isSucceed);
            var isExists = await Database.OnceAsync(async db =>
                await db.Collections.AnyAsync(c => c.Name.Equals(toBeRemoved.Name)));
            Assert.False(isExists);
        }
    }
}
