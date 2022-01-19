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
        public async Task ChangePasswordWithoutOldPassword_ExpectsNoEntryInHistroyTable()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now, PasswordHash = null };

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
        public async Task ChangePassword_ExpectsEntryInHistroyTable()
        {
            string oldPasswordHash;
            User user;
            using (var scope = _serviceProvider.CreateScope())
            {
                var hashingProvider = scope.ServiceProvider.GetRequiredService<IHashingProvider>();
                oldPasswordHash = hashingProvider.Create("oldPassword");
                user = new User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now, PasswordHash = oldPasswordHash };

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
                Assert.AreEqual(oldPasswordHash, passwordHistory.PasswordHash);
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
                var oldPasswordHash = hashingProvider.Create("oldPassword");
                user = new User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now, PasswordHash = oldPasswordHash };

                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.Users.Add(user);

                reusedPassword = "oldPassword1";
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