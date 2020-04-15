using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.Token
{
    public interface IToken
    {
        Task<IDictionary<string, object>> GetPayloadAsync();

        Task ParseAsync(string value);

        Task<string> SerializeAsync();

        Task SetPayloadAsync(IDictionary<string, object> data, string issuer, string audience, ClaimsIdentity subject, string scope, string nonce, TimeSpan expiration);

        Task<bool> ValidateAsync();
    }
}