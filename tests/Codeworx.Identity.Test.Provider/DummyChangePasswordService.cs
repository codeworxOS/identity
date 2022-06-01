namespace Codeworx.Identity.Test
{
    using System.Threading.Tasks;
    using Codeworx.Identity.Model;

    public class DummyChangePasswordService : IChangePasswordService
    {
        public Task SetPasswordAsync(IUser user, string password)
        {
            if (user is DummyUserService.IDummyUser dummyUser)
            {
                dummyUser.ResetPassword(password);
            }

            return Task.CompletedTask;
        }
    }
}