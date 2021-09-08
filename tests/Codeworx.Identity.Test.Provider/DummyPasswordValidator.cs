using Codeworx.Identity.Model;
using System.Threading.Tasks;

namespace Codeworx.Identity.Test
{
    public class DummyPasswordValidator : IPasswordValidator
    {
        public Task<bool> Validate(IUser user, string password)
        {
            if (user is DummyUserService.IDummyUser dummyUser)
            {
                return Task.FromResult(dummyUser.PasswordHash == password);
            }

            return Task.FromResult(false);
        }
    }
}