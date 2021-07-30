using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationCodeGenerator : IAuthorizationCodeGenerator, IDisposable
    {
        private static readonly IReadOnlyCollection<char> _allowedCharacters;
        private readonly System.IDisposable _subscription;
        private bool _disposedValue;
        private AuthorizationCodeOptions _options;

        static AuthorizationCodeGenerator()
        {
            _allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToImmutableArray();
        }

        public AuthorizationCodeGenerator(IOptionsMonitor<AuthorizationCodeOptions> options)
        {
            _options = options.CurrentValue;
            _subscription = options.OnChange(p => _options = p);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<string> GenerateCode()
        {
            var authorizationCodeBuilder = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                var buffer = new byte[_options.Length];
                rng.GetNonZeroBytes(buffer);

                for (int i = 0; i < _options.Length; i++)
                {
                    var index = buffer[i] % _allowedCharacters.Count;
                    var value = _allowedCharacters.ElementAt(index);
                    authorizationCodeBuilder.Append(value);
                }
            }

            var authorizationCode = authorizationCodeBuilder.ToString();

            return Task.FromResult(authorizationCode);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}