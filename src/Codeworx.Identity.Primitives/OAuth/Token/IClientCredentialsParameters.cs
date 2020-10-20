using System;

namespace Codeworx.Identity.OAuth.Token
{
    public interface IClientCredentialsParameters : IIdentityDataParameters
    {
        string ClientSecret { get; }

        TimeSpan TokenExpiration { get; }
    }
}
