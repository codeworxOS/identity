using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityClientService<TContext> : IClientService
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IUserService _userService;

        public EntityClientService(TContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public virtual async Task<IClientRegistration> GetById(string clientIdentifier)
        {
            var id = Guid.Parse(clientIdentifier);

            var result = await _context.Set<ClientConfiguration>()
                .Include(p => p.ValidRedirectUrls)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (result == null)
            {
                return null;
            }

            IUser user = null;

            if (result.UserId.HasValue)
            {
                user = await _userService.GetUserByIdAsync(result.UserId.Value.ToString("N"));
            }

            return new Data.ClientRegistration
            {
                ClientId = result.Id.ToString("N"),
                ClientSecretHash = result.ClientSecretHash,
                ClientType = result.ClientType,
                TokenExpiration = result.TokenExpiration,
                AllowScim = result.AllowScim,
                ValidRedirectUrls = result.ValidRedirectUrls.Select(p => new Uri(p.Url, UriKind.RelativeOrAbsolute)).ToImmutableList(),
                User = user,
                AccessTokenType = result.AccessTokenType,
                AccessTokenTypeConfiguration = result.AccessTokenTypeConfiguration,
                AuthenticationMode = result.AuthenticationMode,
            };
        }
    }
}