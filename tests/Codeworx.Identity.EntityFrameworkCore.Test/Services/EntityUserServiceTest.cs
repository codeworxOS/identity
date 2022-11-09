using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace Codeworx.Identity.EntityFrameworkCore.Test.Services
{
    public class EntityUserServiceTest : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private bool _disposedValue;

        public EntityUserServiceTest()
        {
            var service = new ServiceCollection();

            var databaseId = Guid.NewGuid().ToString("N");

            service.AddScoped<IUserService, EntityUserService<CodeworxIdentityDbContext>>();
            service.AddDbContext<CodeworxIdentityDbContext>(p => p.UseInMemoryDatabase(databaseId));

            _serviceProvider = service.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        protected IServiceScope CreateScope()
        {
            return _serviceProvider.CreateScope();
        }

        [Test]
        public async Task TestGetUserByName_ExistingUser_ExpectsOk()
        {
            var user = await CreateTestUser();

            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();

                var userResponse = await service.GetUserByNameAsync(user.Name);

                Assert.NotNull(userResponse);
                Assert.AreEqual(user.Id, Guid.Parse(userResponse.Identity));
            }
        }

        [Test]
        public async Task TestGetUserByName_NotExistingUser_ExpectsNull()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();

                var userResponse = await service.GetUserByNameAsync("notexistingusername" + Guid.NewGuid());

                Assert.IsNull(userResponse);
            }
        }

        [Test]
        public async Task TestGetUserById_ExistingUser_ExpectsOk()
        {
            var user = await CreateTestUser();

            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();

                var userResponse = await service.GetUserByIdAsync(user.Id.ToString());

                Assert.NotNull(userResponse);
                Assert.AreEqual(user.Id, Guid.Parse(userResponse.Identity));
            }
        }

        [Test]
        public async Task TestGetUserById_NotExistingUser_ExpectsNull()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IUserService>();

                var notExistingUserId = Guid.NewGuid().ToString();
                var userResponse = await service.GetUserByIdAsync(notExistingUserId);

                Assert.IsNull(userResponse);
            }
        }

        private async Task<EntityFrameworkCore.Model.User> CreateTestUser()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = $"testUser{Guid.NewGuid()}", Created = DateTime.Now };

            using (var scope = _serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync();
            }

            return user;
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
