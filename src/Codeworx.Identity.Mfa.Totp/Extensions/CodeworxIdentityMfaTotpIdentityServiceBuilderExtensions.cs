using Codeworx.Identity;
using Codeworx.Identity.AspNetCore;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Configuration.Internal;
using Codeworx.Identity.Login;
using Codeworx.Identity.Mfa.Totp;
using Codeworx.Identity.Mfa.Totp.Binder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityMfaTotpIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder AddMfaTotp(this IIdentityServiceBuilder builder)
        {
            builder.ReplaceService<TotpMfaLoginProcessor, TotpMfaLoginProcessor>(ServiceLifetime.Scoped);
            builder.ReplaceService<IRequestBinder<TotpLoginRequest>, TotpLoginRequestBinder>(ServiceLifetime.Scoped);
            builder.ReplaceService<IRequestValidator<TotpLoginRequest>, TotpLoginRequestValidator>(ServiceLifetime.Scoped);

            builder.RegisterMultiple<IProcessorTypeLookup, TotpMfaLoginProcessorLookup>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, RegisterTotpTemplate>(ServiceLifetime.Singleton);
            builder.RegisterMultiple<IPartialTemplate, LoginTotpTemplate>(ServiceLifetime.Singleton);

            return builder;
        }
    }
}