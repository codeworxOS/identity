using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Codeworx.Identity.Web.Test
{
    public class Program
    {
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public static void Main(string[] args)
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            BuildWebHost(args)
                .MigrateDatabase()
                .Run();
        }
    }
}