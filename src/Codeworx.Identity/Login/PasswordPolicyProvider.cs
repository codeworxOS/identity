using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login
{
    public class PasswordPolicyProvider : IPasswordPolicyProvider
    {
        private readonly IdentityOptions _options;

        public PasswordPolicyProvider(IOptionsSnapshot<IdentityOptions> options)
        {
            _options = options.Value;
        }

        public Task<PasswordPolicy> GetPolicyAsync()
        {
            return Task.FromResult(new PasswordPolicy(_options.PasswordRegex, _options.PasswordDescription));
        }
    }
}
