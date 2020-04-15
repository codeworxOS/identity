using System.Collections.Generic;
using System.Linq;
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
                flows.Add(Codeworx.Identity.OAuth.Constants.GrantType.AuthorizationCode);
            }
            else if (config.Type.HasFlag(ClientType.Native) || config.Type.HasFlag(ClientType.UserAgent))
            {
                flows.Add(Codeworx.Identity.OAuth.Constants.ResponseType.Code);
                flows.Add(Codeworx.Identity.OAuth.Constants.GrantType.AuthorizationCode);
                flows.Add(Codeworx.Identity.OAuth.Constants.ResponseType.Token);
                flows.Add(Codeworx.Identity.OAuth.Constants.GrantType.Password);
            }

            ////else if (config.Type.HasFlag(ClientType.Backend))
            ////{
            ////}
            ////else if (config.Type.HasFlag(ClientType.ApiKey))
            ////{
            ////}

            return new ClientRegistration(id, hash, salt, new ISupportedFlow[] { new SupportedFlow(flows) }, config.TokenExpiration, urls);
        }

        private class SupportedFlow : ISupportedFlow
        {
            private HashSet<string> _flows;

            public SupportedFlow(HashSet<string> flows)
            {
                _flows = flows;
            }

            public bool IsSupported(string flowKey)
            {
                return _flows.Contains(flowKey);
            }
        }
    }
}