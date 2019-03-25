using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeGenerator : IAuthorizationCodeGenerator
    {
        private readonly IOptions<AuthorizationCodeOptions> _options;
        private static readonly IReadOnlyCollection<char> _allowedCharacters;

        static AuthorizationCodeGenerator()
        {
            var allowedCharacters = new List<char>();

            for (var i = '\u0020'; i < '\u007e'; i++)
            {
                allowedCharacters.Add(i);
            }

            _allowedCharacters = allowedCharacters;
        }

        public AuthorizationCodeGenerator(IOptions<AuthorizationCodeOptions> options)
        {
            _options = options;
        }

        #region Public Methods

        public Task<string> GenerateCode(AuthorizationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            var authorizationCodeBuilder = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = 0; i < _options.Value.Length; i++)
                {
                    var randomByte = new byte[1];
                    rng.GetNonZeroBytes(randomByte);

                    var index = randomByte[0] % _allowedCharacters.Count;
                    var value = _allowedCharacters.ElementAt(index);
                    authorizationCodeBuilder.Append(value);
                }
            }

            var authorizationCode = authorizationCodeBuilder.ToString();

            return Task.FromResult(authorizationCode);
        }

        #endregion
    }
}