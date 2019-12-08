using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TodoList.WebServer.Test.Factories;

namespace TodoList.WebServer.Test
{
    public abstract class UnitTestBase : IAsyncDisposable, IDisposable
    {
        private DbConnection _connection;

        protected DbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqliteConnection("DataSource=:memory:");
                    _connection.Open();
                }

                return _connection;
            }
        }

        protected DatabaseFactory Database => new DatabaseFactory(Connection);

        protected TResult Scoped<TResult>(Func<TResult> scoped) => scoped.Invoke();

        public virtual void Dispose()
        {
            DisposeAsync().AsTask().Wait();
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (_connection == null)
                return;

            try
            {
                await _connection.CloseAsync();
            }
            finally
            {
                await _connection.DisposeAsync();
            }
        }
    }
}
