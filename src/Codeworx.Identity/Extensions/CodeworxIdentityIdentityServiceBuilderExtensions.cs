using System.Reflection;
using Codeworx.Identity;
using Codeworx.Identity.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder AddAssets(this IIdentityServiceBuilder builder, Assembly assembly)
        {
            builder.ServiceCollection.AddSingleton<IAssetProvider, AssemblyAssetProvider>(sp => new AssemblyAssetProvider(assembly));
            return builder;
        }
    }
}