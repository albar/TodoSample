using System;
using Microsoft.EntityFrameworkCore;
using TodoList.WebServer.Models;

namespace TodoList.WebServer
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Collection> Collections { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Collection>(entity =>
            {
                entity.HasKey(collection => collection.Id);

                entity.Property(collection => collection.Name)
                    .IsRequired();

                entity.HasMany(collection => collection.TodoItems)
                    .WithOne(item => item.Collection);
            });

            builder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(item => item.Id);

                entity.Property(item => item.Name)
                    .IsRequired();

                entity.Property(item => item.IsDone)
                    .HasDefaultValue(false);
            });
        }
    }
}
