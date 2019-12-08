using System.Net.Http;
using TodoList.WebServer.Test.Factories;

namespace TodoList.WebServer.Test
{
    public abstract class IntegratedTestBase : UnitTestBase
    {
        protected ApplicationFactory App => new ApplicationFactory(Connection);

        protected HttpClient Client => App.CreateClient();
    }
}
