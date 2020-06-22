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

            byte[] salt = null, hash = null;

            if (!string.IsNullOrWhiteSpace(config.Secret))
            {
                salt = hashing.CrateSalt();
                hash = hashing.Hash(config.Secret, salt);
            }

            if (config.Type == ClientType.ApiKey)
            {
                var user = await userService.GetUserByNameAsync(config.User).ConfigureAwait(false);

                if (user == null)
                {
                    throw new AuthenticationException($"The User prvided for client {id} could not be found.");
                }
            }

            ////else if (config.Type.HasFlag(ClientType.Backend))
            ////{
            ////}
            ////else if (config.Type.HasFlag(ClientType.ApiKey))
            ////{
            ////}

            return new ClientRegistration(id, hash, salt, ClientType.Native, config.TokenExpiration, urls);
        }
    }
}