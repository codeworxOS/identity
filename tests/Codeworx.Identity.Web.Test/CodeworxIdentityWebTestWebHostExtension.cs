using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Web.Test
{
    public static class CodeworxIdentityWebTestWebHostExtension
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            var configuration = webHost.Services.GetRequiredService<IConfiguration>();

            webHost.Services.MigrateDatabase(configuration);

            return webHost;
        }
    }
}