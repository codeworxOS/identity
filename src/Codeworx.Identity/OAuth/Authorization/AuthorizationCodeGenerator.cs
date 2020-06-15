using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationCodeGenerator : IAuthorizationCodeGenerator
    {
        private static readonly IReadOnlyCollection<char> _allowedCharacters;

        static AuthorizationCodeGenerator()
        {
            _allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToImmutableArray();
        }

        public Task<string> GenerateCode(IAuthorizationParameters request, int length)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var authorizationCodeBuilder = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[length];
                rng.GetNonZeroBytes(buffer);

                for (int i = 0; i < length; i++)
                {
                    var index = buffer[i] % _allowedCharacters.Count;
                    var value = _allowedCharacters.ElementAt(index);
                    authorizationCodeBuilder.Append(value);
                }
            }

            var authorizationCode = authorizationCodeBuilder.ToString();

            return Task.FromResult(authorizationCode);
        }
    }
}