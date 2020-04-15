using System;
using Codeworx.Identity.EntityFrameworkCore;
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

            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);

            BuildWebHost(args)
                .MigrateDatabase()
                .Run();
        }
    }
}