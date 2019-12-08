using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TodoList.WebServer.Test.Factories
{
    public class ApplicationFactory : WebApplicationFactory<Startup>
    {
        private DbConnection _connection;

        public ApplicationFactory(DbConnection connection)
        {
            _connection = connection;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
        }

        private void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
        {
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        }
    }
}
