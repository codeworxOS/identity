using System.Reflection;
using Codeworx.Identity;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.ExternalLogin;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder AddAssets(this IIdentityServiceBuilder builder, Assembly assembly)
        {
            builder.ServiceCollection.AddSingleton<IAssetProvider, AssemblyAssetProvider>(sp => new AssemblyAssetProvider(assembly));
            builder.ServiceCollection.AddScoped<WindowsLoginProcessor>();
            builder.ServiceCollection.AddScoped<OAuthLoginProcessor>();
            builder.PasswordValidator<PasswordValidator>();
            return builder;
        }
    }
}