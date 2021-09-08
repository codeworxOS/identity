using Codeworx.Identity.Model;
using System.Threading.Tasks;

namespace Codeworx.Identity.Test
{
    public class DummyPasswordValidator : IPasswordValidator
    {
        public Task<bool> Validate(IUser user, string password)
        {
            return Task.FromResult(
                (user.Name == Constants.DefaultAdminUserName && password == Constants.DefaultAdminUserName)
                || (user.Name == Constants.MultiTenantUserName && password == Constants.MultiTenantUserName)
                || (user.Name == Constants.ForcePasswordUserName && password == Constants.ForcePasswordUserName));
        }
    }
}