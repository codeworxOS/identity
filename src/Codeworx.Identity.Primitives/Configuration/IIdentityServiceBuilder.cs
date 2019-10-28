using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration
{
    public interface IIdentityServiceBuilder
    {
        IServiceCollection ServiceCollection { get; }
    }
}