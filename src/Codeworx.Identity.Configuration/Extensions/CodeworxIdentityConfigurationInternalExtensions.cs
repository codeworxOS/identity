using System.Threading.Tasks;
using Codeworx.Identity.Configuration.Infrastructure;
using Codeworx.Identity.Configuration.Model;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;

namespace Codeworx.Identity.Configuration.Extensions
{
    public static class CodeworxIdentityConfigurationInternalExtensions
    {
        public static async Task<ClientRegistration> ToRegistration(this ClientConfig config, IUserService userService, IStringResources stringResources, string id)
        {
            var urls = config.RedirectUris;
            IUser user = null;

            if (config.Type == ClientType.ApiKey)
            {
                user = await userService.GetUserByNameAsync(config.User).ConfigureAwait(false);

                if (user == null)
                {
                    var message = string.Format(stringResources.GetResource(StringResource.UserForClientP0NotFoundError), id);
                    throw new AuthenticationException(message);
                }
            }

            ////else if (config.Type.HasFlag(ClientType.Backend))
            ////{
            ////}
            ////else if (config.Type.HasFlag(ClientType.ApiKey))
            ////{
            ////}

            return new ClientRegistration(id, config.Secret, ClientType.Native, config.TokenExpiration, config.AccessTokenType, config.AccessTokenTypeConfiguration, urls, user, config.AllowScim, config.RefreshTokenLifetime, config.RefreshTokenExpiration);
        }
    }
}