using System.Threading.Tasks;

namespace Codeworx.Identity.Test
{
    using Codeworx.Identity.Model;

    public class DummyFailedLoginService : IFailedLoginService
    {
        public Task SetFailedLoginAsync(IUser user)
        {
            if (user is DummyUserService.IDummyUser dummyUser)
            {
                dummyUser.FailedLoginCount++;
            }

            return Task.CompletedTask;
        }

        public async Task<IUser> ResetFailedLoginsAsync(IUser user)
        {
            if (user is DummyUserService.IDummyUser dummyUser)
            {
                dummyUser.FailedLoginCount = 0;
            }

            return await Task.FromResult(user).ConfigureAwait(false);
        }
    }
}