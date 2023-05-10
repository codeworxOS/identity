using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore
{
    // EventIds 156xx
    public partial class EntityCacheCleanup<TContext> : BackgroundService
        where TContext : DbContext
    {
        private readonly ILogger<EntityCacheCleanup<TContext>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDisposable _subscription;
        private IdentityOptions _options;

        public EntityCacheCleanup(IOptionsMonitor<IdentityOptions> optionsMonitor, IServiceProvider serviceProvider, ILogger<EntityCacheCleanup<TContext>> logger)
        {
            _subscription = optionsMonitor.OnChange(p => _options = p);
            _options = optionsMonitor.CurrentValue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override void Dispose()
        {
            base.Dispose();
            _subscription.Dispose();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_options.Cache.CleanupInterval);
                await using (var scope = _serviceProvider.CreateAsyncScope())
                {
                    CleanupStarted(_logger);
                    try
                    {
                        var now = DateTime.UtcNow;
                        var validUntil = now.Subtract(_options.Cache.RetentionPeriod);

                        var ctx = scope.ServiceProvider.GetRequiredService<TContext>();
                        await using (var transaction = await ctx.Database.BeginTransactionAsync())
                        {
#if EF6
                            var invitations = await ctx.Set<UserInvitation>().Where(p => p.IsDisabled && p.ValidUntil > now)
                                            .Select(p => new UserInvitation { InvitationCode = p.InvitationCode, ValidUntil = p.ValidUntil }).ToListAsync().ConfigureAwait(false);
                            var cache = await ctx.Set<IdentityCache>().Where(p => p.Disabled && p.ValidUntil > now)
                                            .Select(p => new IdentityCache { Key = p.Key, ValidUntil = p.ValidUntil }).ToListAsync().ConfigureAwait(false);

                            ctx.AttachRange(invitations);
                            ctx.AttachRange(cache);
                            invitations.ForEach(p => p.ValidUntil = now);
                            cache.ForEach(p => p.ValidUntil = now);
                            await ctx.SaveChangesAsync().ConfigureAwait(false);

                            invitations = await ctx.Set<UserInvitation>().Where(p => p.ValidUntil < validUntil)
                                            .Select(p => new UserInvitation { InvitationCode = p.InvitationCode }).ToListAsync().ConfigureAwait(false);
                            cache = await ctx.Set<IdentityCache>().Where(p => p.ValidUntil < validUntil)
                                            .Select(p => new IdentityCache { Key = p.Key }).ToListAsync().ConfigureAwait(false);

                            ctx.RemoveRange(invitations);
                            ctx.RemoveRange(cache);
                            await ctx.SaveChangesAsync().ConfigureAwait(false);
#else
                            await ctx.Set<UserInvitation>().Where(p => p.IsDisabled && p.ValidUntil > now).ExecuteUpdateAsync(x => x.SetProperty(p => p.ValidUntil, now)).ConfigureAwait(false);
                            await ctx.Set<IdentityCache>().Where(p => p.Disabled && p.ValidUntil > now).ExecuteUpdateAsync(x => x.SetProperty(p => p.ValidUntil, now)).ConfigureAwait(false);

                            await ctx.Set<UserInvitation>().Where(p => p.ValidUntil < validUntil).ExecuteDeleteAsync().ConfigureAwait(false);
                            await ctx.Set<IdentityCache>().Where(p => p.ValidUntil < validUntil).ExecuteDeleteAsync().ConfigureAwait(false);
#endif
                            await transaction.CommitAsync().ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        CleanupError(_logger, ex);
                    }
                    finally
                    {
                        CleanupCompleted(_logger);
                    }
                }
            }
        }

        [LoggerMessage(EventId = 15602, Level = LogLevel.Information, Message = "Cache cleanup completed for context.")]
        static partial void CleanupCompleted(ILogger logger);

        [LoggerMessage(EventId = 15603, Level = LogLevel.Error, Message = "Error while running cache cleanup job.")]
        static partial void CleanupError(ILogger logger, Exception ex);

        [LoggerMessage(EventId = 15601, Level = LogLevel.Information, Message = "Cache cleanup started.")]
        static partial void CleanupStarted(ILogger logger);
    }
}
