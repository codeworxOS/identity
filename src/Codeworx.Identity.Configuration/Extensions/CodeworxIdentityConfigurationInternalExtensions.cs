using System.Collections.Generic;
using Codeworx.Identity.Configuration.Infrastructure;
using Codeworx.Identity.Configuration.Model;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Extensions
{
    public static class CodeworxIdentityConfigurationInternalExtensions
    {
        public static ClientRegistration ToRegistration(this ClientConfig config, IHashingProvider hashing, string id)
        {
            var urls = config.RedirectUris;

            byte[] salt = null, hash = null;

            if (!string.IsNullOrWhiteSpace(config.Secret))
            {
                salt = hashing.CrateSalt();
                hash = hashing.Hash(config.Secret, salt);
            }

            var flows = new HashSet<string>();

            if (config.Type.HasFlag(ClientType.Web))
            {
                flows.Add(Constants.OAuth.GrantType.AuthorizationCode);
            }
            else if (config.Type.HasFlag(ClientType.Native) || config.Type.HasFlag(ClientType.UserAgent))
            {
                flows.Add(Constants.OAuth.ResponseType.Code);
                flows.Add(Constants.OAuth.GrantType.AuthorizationCode);
                flows.Add(Constants.OAuth.ResponseType.Token);
                flows.Add(Constants.OAuth.GrantType.Password);
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