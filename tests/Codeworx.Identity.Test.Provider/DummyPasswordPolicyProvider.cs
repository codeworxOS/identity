namespace Codeworx.Identity.Test.Provider
{
    using System.Threading.Tasks;
    using Codeworx.Identity.Login;
    using Codeworx.Identity.Model;

    public class DummyPasswordPolicyProvider : IPasswordPolicyProvider
    {
        public const string Description = "Password should be 1111 or admin";

        public Task<PasswordPolicy> GetPolicyAsync()
        {
            return Task.FromResult(new PasswordPolicy("1111|admin", Description));
        }
    }
}
