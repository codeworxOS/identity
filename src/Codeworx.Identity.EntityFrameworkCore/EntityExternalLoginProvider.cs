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
        private readonly IEnumerable<IProcessorTypeLookup> _processorTypeLookups;
        private readonly IServiceProvider _serviceProvider;
        private readonly bool _windowsAuthenticationEnabled;

        public EntityExternalLoginProvider(IOptionsSnapshot<IdentityOptions> options, TContext context, IEnumerable<IProcessorTypeLookup> processorTypeLookups, IServiceProvider serviceProvider)
        {
            _context = context;
            _processorTypeLookups = processorTypeLookups;
            _serviceProvider = serviceProvider;
            _windowsAuthenticationEnabled = options.Value.WindowsAuthenticationEnabled;
        }

        public async Task<IEnumerable<IExternalLoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
        {
            var authenticationProviderQuery = _context.Set<ExternalAuthenticationProvider>() as IQueryable<ExternalAuthenticationProvider>;

            if (userName != null)
            {
                var user = await _context.Set<User>().SingleOrDefaultAsync(p => p.Name == userName);
                var userId = user?.Id;

                authenticationProviderQuery = authenticationProviderQuery.Where(p => p.Users.Any(t => t.RightHolderId == userId));
            }

            if (_windowsAuthenticationEnabled == false)
            {
                authenticationProviderQuery = authenticationProviderQuery.Where(p => p.Id != Guid.Parse(Constants.ExternalWindowsProviderId));
            }

            var loginRegistrations = await authenticationProviderQuery
                                               .ToListAsync();

            var result = loginRegistrations
                            .Select(p => p.ToExternalLoginRegistration(_processorTypeLookups, _serviceProvider))
                            .ToList();

            return result;
        }
    }
}