using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class LoginRegistrationProvider<TContext> : ILoginRegistrationProvider
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IEnumerable<IProcessorTypeLookup> _processorTypeLookups;
        private readonly IServiceProvider _serviceProvider;

        public LoginRegistrationProvider(TContext context, IEnumerable<IProcessorTypeLookup> processorTypeLookups, IServiceProvider serviceProvider)
        {
            _context = context;
            _processorTypeLookups = processorTypeLookups;
            _serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
        {
            var authenticationProviderQuery = _context.Set<AuthenticationProvider>() as IQueryable<AuthenticationProvider>;

            authenticationProviderQuery = authenticationProviderQuery.Where(p => p.Enabled);

            var loginRegistrations = await authenticationProviderQuery
                                                .OrderBy(p => p.SortOrder)
                                               .ToListAsync();

            var result = loginRegistrations
                            .Select(p => p.ToExternalLoginRegistration(_processorTypeLookups, _serviceProvider))
                            .ToList();

            return result;
        }
    }
}