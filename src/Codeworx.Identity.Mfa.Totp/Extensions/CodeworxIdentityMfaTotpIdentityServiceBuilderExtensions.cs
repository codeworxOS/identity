using Codeworx.Identity;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Configuration.Internal;
using Codeworx.Identity.Login;
using Codeworx.Identity.Mfa.Totp;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityMfaTotpIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder AddMfaTotp(this IIdentityServiceBuilder builder)
        {
            builder.ReplaceService<TotpMfaLoginProcessor, TotpMfaLoginProcessor>(ServiceLifetime.Scoped);
            builder.RegisterMultiple<IProcessorTypeLookup, TotpMfaLoginProcessorLookup>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, RegisterTotpTemplate>(ServiceLifetime.Singleton);

            return builder;
        }
    }
}