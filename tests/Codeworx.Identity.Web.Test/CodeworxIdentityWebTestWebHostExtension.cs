using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace Codeworx.Identity.Web.Test
{
    public static class CodeworxIdentityWebTestWebHostExtension
    {
        public static IWebHost MigrateDatabase(this IWebHost webHost)
        {
            webHost.Services.MigrateDatabase();

            return webHost;
        }
    }
}