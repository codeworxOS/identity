using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login
{
    public class DefaultPasswordPolicyProvider : IPasswordPolicyProvider, IDisposable
    {
        private readonly IDisposable _subscription;
        private bool _disposedValue;
        private IdentityOptions _options;
        private RegexPolicy _policy;

        public DefaultPasswordPolicyProvider(IOptionsMonitor<IdentityOptions> options)
        {
            _subscription = options.OnChange(OnOptionsChange);
            _options = options.CurrentValue;
            _policy = GetPolicy(_options);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<IStringPolicy> GetPolicyAsync()
        {
            return Task.FromResult<IStringPolicy>(_policy);
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

        private static RegexPolicy GetPolicy(IdentityOptions options)
        {
            if (options.Password != null)
            {
                return new RegexPolicy(options.Password.Regex, options.Password.Description);
            }

            return new RegexPolicy(
                Constants.DefaultPasswordRegex,
                new Dictionary<string, string>
                {
                    { "de", Constants.DefaultPasswordDescriptionDe },
                    { "en", Constants.DefaultPasswordDescriptionEn },
                });
        }

        private void OnOptionsChange(IdentityOptions options)
        {
            _options = options;
            _policy = GetPolicy(_options);
        }
    }
}
