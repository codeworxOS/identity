using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.ExternalLogin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityExternalLoginProvider<TContext> : IExternalLoginProvider
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly bool _isEnabled;

        public EntityExternalLoginProvider(IOptionsSnapshot<IdentityOptions> options, TContext context)
        {
            _context = context;
            _isEnabled = options.Value.WindowsAuthenticationEnabled;
        }

        public async Task<IEnumerable<IExternalLoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
        {
            var authenticationProviderQuery = _context.Set<AuthenticationProviderUser>() as IQueryable<AuthenticationProviderUser>;

            if (userName != null)
            {
                var user = await _context.Set<User>().SingleOrDefaultAsync(p => p.Name == userName);
                var userId = user?.Id;

                authenticationProviderQuery = authenticationProviderQuery.Where(p => p.RightHolderId == userId);
            }

            if (_isEnabled == false)
            {
                authenticationProviderQuery = authenticationProviderQuery.Where(p => p.Provider.Id != Guid.Parse(Constants.ExternalWindowsProviderId));
            }

            var authenticationProvider = await authenticationProviderQuery.Select(p => p.Provider).ToListAsync();

            return authenticationProvider;
        }
    }
}