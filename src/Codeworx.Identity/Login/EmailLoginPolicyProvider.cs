using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;

namespace Codeworx.Identity.Login
{
    public class EmailLoginPolicyProvider : ILoginPolicyProvider
    {
        private static readonly Regex _emailRegex;
        private readonly IStringResources _resources;

        static EmailLoginPolicyProvider()
        {
            _emailRegex = new Regex(Constants.EmailRegex, RegexOptions.IgnoreCase);
        }

        public EmailLoginPolicyProvider(IStringResources resources)
        {
            _resources = resources;
        }

        public Task<IStringPolicy> GetPolicyAsync()
        {
            var policy = new RegexPolicy(
                _emailRegex,
                new Dictionary<string, string>
                {
                    { _resources.GetResource(StringResource.LanguageCode), _resources.GetResource(StringResource.EmailLoginDescription) },
                });

            return Task.FromResult<IStringPolicy>(policy);
        }
    }
}
