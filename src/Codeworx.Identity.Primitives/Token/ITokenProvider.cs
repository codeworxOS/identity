using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.Token
{
    public interface ITokenProvider
    {
        Type ConfigurationType { get; }

        string TokenType { get; }

        Task<IToken> CreateAsync(object configuration);
    }
}