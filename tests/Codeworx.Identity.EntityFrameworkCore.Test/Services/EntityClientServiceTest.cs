using System;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Services
{
    public class EntityClientServiceTest : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private bool _disposedValue;

        public EntityClientServiceTest()
        {
            var service = new ServiceCollection();

            var databaseId = Guid.NewGuid().ToString("N");

            service.AddScoped<IClientService, EntityClientService<CodeworxIdentityDbContext>>();
            service.AddScoped<IUserService, EntityUserService<CodeworxIdentityDbContext>>();
            service.AddDbContext<CodeworxIdentityDbContext>(p => p.UseInMemoryDatabase(databaseId));

            _serviceProvider = service.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        protected IServiceScope CreateScope()
        {
            return _serviceProvider.CreateScope();
        }

        [Test]
        public async Task GetApiKeyClient_ExpectsOk()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now };
            var client = new EntityFrameworkCore.Model.ClientConfiguration { Id = Guid.NewGuid(), ClientType = Model.ClientType.ApiKey, UserId = user.Id };

            using (var scope = _serviceProvider.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.Users.Add(user);
                ctx.ClientConfigurations.Add(client);
                await ctx.SaveChangesAsync();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IClientService>();

                var clientResponse = await service.GetById(client.Id.ToString("N"));

                Assert.NotNull(clientResponse);
                Assert.NotNull(clientResponse.User);
                Assert.AreEqual(user.Id.ToString("N"), clientResponse.User.Identity);
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