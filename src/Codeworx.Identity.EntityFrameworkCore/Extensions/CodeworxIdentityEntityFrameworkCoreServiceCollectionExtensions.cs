using Codeworx.Identity.EntityFrameworkCore.Internal;

using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestEntityCache(this IServiceCollection services)
        {
            return services.AddScoped<IRequestEntityCache, RequestEntityCache>();
        }
    }
}