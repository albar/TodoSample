using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;
using TodoList.WebServer.Repositories;
using Xunit;

namespace TodoList.WebServer.Test.Unit
{
    public class TodoItemRepositoryTest : UnitTestBase
    {
        [Fact]
        public async Task Can_Find_A_Todo_Item()
        {
            //Given
            var collection = new Collection { Name = "A Collection" };
            var toBeFound = await Database.OnceAndSaveAsync(async db =>
            {
                collection.TodoItems.Add(new TodoItem { Name = "Item1" });
                collection.TodoItems.Add(new TodoItem { Name = "Item2" });
                await db.Collections.AddAsync(collection);
                return collection.TodoItems.First();
            });

            //When
            var found = await Database.OnceAsync(async db =>
            {
                var repository = new TodoItemRepository(db);
                return await repository.FindAsync(toBeFound.Id);
            });

            //Then
            Assert.Equal(toBeFound.Id, found.Id);
            Assert.Equal(toBeFound.Name, found.Name);
            Assert.Equal(collection.Id, found.Collection.Id);
            Assert.Equal(collection.Name, found.Collection.Name);
        }

        [Fact]
        public async Task Can_Add_A_Todo_Item_To_A_Collection()
        {
            //Given
            var collection = new Collection { Name = "A Collection" };
            await Database.OnceAndSaveAsync(async db => await db.Collections.AddAsync(collection));
            var item = new TodoItem { Name = "An Item" };

            //When
            var result = await Database.OnceAsync(async db =>
            {
                var repository = new TodoItemRepository(db);
                return await repository.AddAsync(collection, item);
            });

            //Then
            Assert.Equal(item.Name, result.Name);
            Assert.False(item.IsDone);
            var isExists = await Database.OnceAsync(async db =>
                await Database.Create().Collections.AnyAsync(
                    c => c.Id == collection.Id && c.TodoItems.Any(i => i.Name == item.Name)));
            Assert.True(isExists);
        }

        [Fact]
        public void Should_Specify_Name_When_Adding_New_Todo_Item()
        {
            //Given

            //When

            //Then
        }

        [Fact]
        public async Task Can_Change_Todo_Item_State()
        {
            //Given
            var collection = new Collection { Name = "A Collection" };
            var item = new TodoItem { Name = "An Item" };
            collection.TodoItems.Add(item);
            await Database.OnceAndSaveAsync(async db => await db.Collections.AddAsync(collection));
            await using var db = Database.Create();
            var repository = new TodoItemRepository(db);

            // Then
            Assert.False(item.IsDone);

            //When
            var isSucceed = await repository.DoneAsync(item);

            //Then
            Assert.True(isSucceed);
            Assert.True(item.IsDone);

            // When
            var isSucceed2 = await repository.UndoneAsync(item);

            // Then
            Assert.True(isSucceed2);
            Assert.False(item.IsDone);
        }

        [Fact]
        public async Task Can_Update_A_Todo_Item()
        {
            //Given
            const string newName = "New Item Name";

            var item = await Database.OnceAndSaveAsync(async db =>
            {
                var collection = new Collection { Name = "A Collection" };
                var item = new TodoItem { Name = "Item1" };
                collection.TodoItems.Add(item);
                await db.Collections.AddAsync(collection);
                return item;
            });

            //When
            var isSucceed = await Database.OnceAsync(async db =>
            {
                var repository = new TodoItemRepository(db);
                item.Name = newName;
                return await repository.UpdateAsync(item);
            });
            Assert.True(isSucceed);

            //Then
            var result = await Database.OnceAsync(async db => await db.TodoItems.FindAsync(item.Id));
            Assert.Equal(newName, result.Name);
        }

        [Fact]
        public async Task Can_Remove_A_Todo_Item()
        {
            //Given
            var item = await Database.OnceAndSaveAsync(async db =>
            {
                var item = new TodoItem { Name = "An Item" };
                var collection = new Collection { Name = "A Collection" };
                collection.TodoItems.Add(item);
                await db.Collections.AddAsync(collection);
                return item;
            });

            //When
            var isSucceed = await Database.OnceAsync(async db =>
            {
                var repository = new TodoItemRepository(db);
                return await repository.RemoveAsync(item);
            });

            //Then
            Assert.True(isSucceed);
            var isExists = await Database.OnceAsync(db =>
                db.TodoItems.AnyAsync(i => i.Name.Equals(item.Name) || i.Id.Equals(item.Id)));
            Assert.False(isExists);
        }
    }
}
