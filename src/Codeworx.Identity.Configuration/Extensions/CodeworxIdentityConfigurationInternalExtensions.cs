using System.Threading.Tasks;
using Codeworx.Identity.Configuration.Infrastructure;
using Codeworx.Identity.Configuration.Model;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Extensions
{
    public static class CodeworxIdentityConfigurationInternalExtensions
    {
        public static async Task<ClientRegistration> ToRegistration(this ClientConfig config, IUserService userService, IHashingProvider hashing, string id)
        {
            var urls = config.RedirectUris;
            IUser user = null;

            if (config.Type == ClientType.ApiKey)
            {
                user = await userService.GetUserByNameAsync(config.User).ConfigureAwait(false);

                if (user == null)
                {
                    throw new AuthenticationException($"The User provided for client {id} could not be found.");
                }
            }

            ////else if (config.Type.HasFlag(ClientType.Backend))
            ////{
            ////}
            ////else if (config.Type.HasFlag(ClientType.ApiKey))
            ////{
            ////}

            return new ClientRegistration(id, config.Secret, ClientType.Native, config.TokenExpiration, urls, user);
        }
    }
}