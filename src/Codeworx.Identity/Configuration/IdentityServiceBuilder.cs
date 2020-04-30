using System.Reflection;
using Codeworx.Identity.ContentType;
using Codeworx.Identity.Login;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public class IdentityServiceBuilder : IIdentityServiceBuilder
    {
        public IdentityServiceBuilder(IServiceCollection collection)
        {
            ServiceCollection = collection;

            this.ReplaceService<IContentTypeLookup, ContentTypeLookup>(ServiceLifetime.Singleton);
            this.ReplaceService<IContentTypeProvider, DefaultContentTypeProvider>(ServiceLifetime.Singleton);

            this.AddAssets(typeof(DefaultViewTemplate).GetTypeInfo().Assembly);
            this.View<DefaultViewTemplate>();

            this.ReplaceService<ILoginViewService, LoginViewService>(ServiceLifetime.Scoped);
            this.ReplaceService<IExternalLoginService, ExternalLoginService>(ServiceLifetime.Scoped);
            this.ReplaceService<IIdentityService, IdentityService>(ServiceLifetime.Scoped);
        }

        public IServiceCollection ServiceCollection { get; }
    }
}