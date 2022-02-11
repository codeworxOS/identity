using System;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Test.Services
{
    using System.Linq;
    using Codeworx.Identity.EntityFrameworkCore.Model;
    using Codeworx.Identity.Resources;

    public class EntityChangePasswordServiceTest : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private bool _disposedValue;

        public EntityChangePasswordServiceTest()
        {
            var service = new ServiceCollection();

            var databaseId = Guid.NewGuid().ToString("N");

            service.AddScoped<IChangePasswordService, EntityChangePasswordService<CodeworxIdentityDbContext>>();
            service.AddScoped<IStringResources, DefaultStringResources>();
            service.AddScoped<IHashingProvider, DummyHashingProvider>();
            service.AddOptions();
            service.AddSingleton<IConfigureOptions<IdentityOptions>>(
                sp => new ConfigureOptions<IdentityOptions>(i => i.PasswordHistoryLength = 3));

            service.AddDbContext<CodeworxIdentityDbContext>(p => p.UseInMemoryDatabase(databaseId));

            _serviceProvider = service.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        [Test]
        public async Task ChangePasswordWithoutOldPassword_ExpectsNoEntryInHistoryTable()
        {
            var user = new User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now, PasswordHash = null };

            using (var scope = _serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var changePasswordService = scope.ServiceProvider.GetRequiredService<IChangePasswordService>();

                await changePasswordService.SetPasswordAsync(
                    new EntityFrameworkCore.Data.User { Identity = user.Id.ToString("N") },
                    "newPassword");
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                var hasHistoryEntry = await ctx.UserPasswordHistory.Where(x => x.UserId == user.Id).AnyAsync();
                Assert.AreEqual(false, hasHistoryEntry);
            }
        }

        [Test]
        public async Task ChangePassword_ExpectsEntryInHistoryTable()
        {
            string initialPasswordHash;
            User user;
            using (var scope = _serviceProvider.CreateScope())
            {
                var hashingProvider = scope.ServiceProvider.GetRequiredService<IHashingProvider>();
                initialPasswordHash = hashingProvider.Create("initial");
                user = new User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now, PasswordHash = initialPasswordHash };

                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var changePasswordService = scope.ServiceProvider.GetRequiredService<IChangePasswordService>();

                await changePasswordService.SetPasswordAsync(
                    new EntityFrameworkCore.Data.User { Identity = user.Id.ToString("N") },
                    "newPassword");
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                var passwordHistory = await ctx.UserPasswordHistory.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();
                Assert.AreEqual(initialPasswordHash, passwordHistory.PasswordHash);
            }
        }

        [Test]
        public async Task PasswordReuse_ExpectsException()
        {
            User user;
            string reusedPassword;
            using (var scope = _serviceProvider.CreateScope())
            {
                var hashingProvider = scope.ServiceProvider.GetRequiredService<IHashingProvider>();
                var initialPasswordHash = hashingProvider.Create("initial");
                user = new User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now, PasswordHash = initialPasswordHash };

                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.Users.Add(user);

                reusedPassword = "reusedPassword";
                var oldPasswordHash1 = hashingProvider.Create(reusedPassword);
                ctx.UserPasswordHistory.Add(
                    new UserPasswordHistory { ChangedAt = DateTime.UtcNow, PasswordHash = oldPasswordHash1, UserId = user.Id });

                var oldPasswordHash2 = hashingProvider.Create("oldPassword2");
                ctx.UserPasswordHistory.Add(
                    new UserPasswordHistory { ChangedAt = DateTime.UtcNow, PasswordHash = oldPasswordHash2, UserId = user.Id });

                await ctx.SaveChangesAsync();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var changePasswordService = scope.ServiceProvider.GetRequiredService<IChangePasswordService>();

                Assert.ThrowsAsync<PasswordChangeException>(
                    () => changePasswordService.SetPasswordAsync(
                        new EntityFrameworkCore.Data.User { Identity = user.Id.ToString("N") },
                        reusedPassword));
            }
        }

        [Test]
        public async Task PasswordReuseOutOfHistory_ExpectsException()
        {
            User user;
            string reusedPassword;
            using (var scope = _serviceProvider.CreateScope())
            {
                var hashingProvider = scope.ServiceProvider.GetRequiredService<IHashingProvider>();
                var identityOptions = scope.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>().Value;

                var initialPasswordHash = hashingProvider.Create("initial");
                var date = DateTime.UtcNow;
                user = new User { Id = Guid.NewGuid(), Name = "test", Created = date, PasswordHash = initialPasswordHash };

                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.Users.Add(user);

                reusedPassword = "reusedPassword";
                var reusedPasswordHash = hashingProvider.Create("reusedPassword");
                ctx.UserPasswordHistory.Add(
                    new UserPasswordHistory { ChangedAt = date, PasswordHash = reusedPasswordHash, UserId = user.Id });

                for (var i = 0; i < identityOptions.PasswordHistoryLength; i++)
                {
                    var oldPasswordHash = hashingProvider.Create($"oldPassword{i}");
                    ctx.UserPasswordHistory.Add(
                        new UserPasswordHistory { ChangedAt = date.AddSeconds(i + 1), PasswordHash = oldPasswordHash, UserId = user.Id });
                }

                await ctx.SaveChangesAsync();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var changePasswordService = scope.ServiceProvider.GetRequiredService<IChangePasswordService>();

                Assert.DoesNotThrowAsync(
                    () => changePasswordService.SetPasswordAsync(
                        new EntityFrameworkCore.Data.User { Identity = user.Id.ToString("N") },
                        reusedPassword));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _serviceProvider.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}