namespace Codeworx.Identity.Test.Provider
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Codeworx.Identity.Login;
    using Codeworx.Identity.Model;

    public class DummyPasswordPolicyProvider : IPasswordPolicyProvider
    {
        public static IDictionary<string, string> Description = new Dictionary<string, string>
        {
            { "en", "Password should be 1111 or admin" }
        };

        public Task<IStringPolicy> GetPolicyAsync()
        {
            return Task.FromResult<IStringPolicy>(new RegexPolicy("1111|admin", Description));
        }
    }
}
