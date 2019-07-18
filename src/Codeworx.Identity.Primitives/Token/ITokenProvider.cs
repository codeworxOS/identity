using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.Token
{
    public interface ITokenProvider
    {
        string TokenType { get; }

        Type ConfigurationType { get; }

        Task<IToken> CreateAsync(object configuration, DateTime expiration);
    }
}
