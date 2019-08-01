using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Model
{
    public interface IClientRegistration
    {
        string ClientId { get; }

        byte[] ClientSecretHash { get; }

        byte[] ClientSecretSalt { get; }

        IReadOnlyList<ISupportedFlow> SupportedFlow { get; }

        IReadOnlyList<string> ValidRedirectUrls { get; }

        Uri DefaultRedirectUri { get; }
    }
}
