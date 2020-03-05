using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.Token
{
    public interface IToken
    {
        Task<IDictionary<string, object>> GetPayloadAsync();

        Task ParseAsync(string value);

        Task<string> SerializeAsync();

        Task SetPayloadAsync(IDictionary<string, object> claims, TimeSpan expiration);

        Task<bool> ValidateAsync();
    }
}