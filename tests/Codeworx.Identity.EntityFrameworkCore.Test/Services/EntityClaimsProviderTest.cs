using System;
using System.Collections.Frozen;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Services
{
    public class EntityClaimsProviderTest : IDisposable
    {
        private ServiceProvider _serviceProvider;
        private bool _disposedValue;

        [SetUp]
        public void Setup()
        {
            var service = new ServiceCollection();

            var databaseId = Guid.NewGuid().ToString("N");

            service.AddRequestEntityCache();
            service.AddScoped<IClaimsProvider, EntityClaimsProvider<CodeworxIdentityDbContext>>();
            service.AddScoped<IUserService, EntityUserService<CodeworxIdentityDbContext>>();
            service.AddScoped<ITenantService, EntityTenantService<CodeworxIdentityDbContext>>();
            service.AddScoped<IScopeProvider, EntityScopeProvider<CodeworxIdentityDbContext>>();
            service.AddScoped<ISystemClaimsProvider, SystemClaimsProvider<CodeworxIdentityDbContext>>();
            service.AddScoped<IClaimsService, ClaimsService>();
            service.AddDbContext<CodeworxIdentityDbContext>(p => p.UseInMemoryDatabase(databaseId));

            _serviceProvider = service.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        }

        protected IServiceScope CreateScope()
        {
            return _serviceProvider.CreateScope();
        }

        [Test]
        public async Task GetGlobalClaims_ExpectsOk()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now };
            var client = new EntityFrameworkCore.Model.ClientConfiguration { Id = Guid.NewGuid(), ClientType = Model.ClientType.Native };
            var scope = new EntityFrameworkCore.Model.Scope { Id = Guid.NewGuid(), ScopeKey = "testscope" };
            var scopeAssignment = new EntityFrameworkCore.Model.ScopeAssignment { ClientId = client.Id, ScopeId = scope.Id };
            var claimType = new EntityFrameworkCore.Model.ClaimType { Id = Guid.NewGuid(), Target = ClaimTarget.AllTokens, TypeKey = "testclaim" };
            var scopeClaim = new EntityFrameworkCore.Model.ScopeClaim { ScopeId = scope.Id, ClaimTypeId = claimType.Id };
            var claimValue = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, Value = "claimValue" };

            using (var spScope = _serviceProvider.CreateScope())
            {

                var ctx = spScope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.AddRange(user, client, scope, scopeAssignment, claimType, scopeClaim, claimValue);
                await ctx.SaveChangesAsync();
            }

            using (var spScope = _serviceProvider.CreateScope())
            {
                var service = spScope.ServiceProvider.GetRequiredService<IClaimsProvider>();
                var userService = spScope.ServiceProvider.GetRequiredService<IUserService>();

                var request = new AuthorizationRequest(client.Id.ToString("N"), null, Constants.OAuth.ResponseType.Token, null, null);
                var u = await userService.GetUserByIdAsync(user.Id.ToString("N"));
                var identity = GetClaimsIdentityFromUser(u);

                IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, identity, u);
                builder
                    .WithScopes("openid", "testscope");

                var claims = await service.GetClaimsAsync(builder.Parameters);

                Assert.AreEqual(1, claims.Count());
                var result = claims.First();
                Assert.AreEqual("testclaim", result.Type.First());

                Assert.AreEqual(1, result.Values.Count());
                Assert.AreEqual("claimValue", result.Values.First());
            }
        }


        [Test]
        public async Task GetGlobalGroupClaims_ExpectsOk()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now };
            var globalGroup = new EntityFrameworkCore.Model.Group { Id = Guid.NewGuid(), Name = "global" };
            var tenant1Group = new EntityFrameworkCore.Model.Group { Id = Guid.NewGuid(), Name = "tenant1" };
            var tenant2Group = new EntityFrameworkCore.Model.Group { Id = Guid.NewGuid(), Name = "tenant2" };

            var tenant1 = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant1" };
            var tenant2 = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant2" };

            var tenant1User = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = user.Id, TenantId = tenant1.Id };
            var tenant2User = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = user.Id, TenantId = tenant2.Id };

            var tenant1GroupAssignment = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = tenant1Group.Id, TenantId = tenant1.Id };
            var tenant2GroupAssignment = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = tenant2Group.Id, TenantId = tenant2.Id };

            var member1 = new EntityFrameworkCore.Model.RightHolderGroup { GroupId = globalGroup.Id, RightHolderId = user.Id };
            var member2 = new EntityFrameworkCore.Model.RightHolderGroup { GroupId = tenant1Group.Id, RightHolderId = user.Id };
            var member3 = new EntityFrameworkCore.Model.RightHolderGroup { GroupId = tenant2Group.Id, RightHolderId = user.Id };


            var client = new EntityFrameworkCore.Model.ClientConfiguration { Id = Guid.NewGuid(), ClientType = Model.ClientType.Native };


            using (var spScope = _serviceProvider.CreateScope())
            {

                var ctx = spScope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.AddRange(user, client, globalGroup, tenant1Group, tenant2Group, tenant1, tenant2, tenant1User, tenant2User, tenant1GroupAssignment, tenant2GroupAssignment, member1, member2, member3);
                await ctx.SaveChangesAsync();
            }

            using (var spScope = _serviceProvider.CreateScope())
            {
                var service = spScope.ServiceProvider.GetRequiredService<IClaimsService>();
                var userService = spScope.ServiceProvider.GetRequiredService<IUserService>();

                var request = new AuthorizationRequest(client.Id.ToString("N"), null, Constants.OAuth.ResponseType.Token, null, null);
                var u = await userService.GetUserByIdAsync(user.Id.ToString("N"));
                var identity = GetClaimsIdentityFromUser(u);

                IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, identity, u);
                builder
                    .WithScopes("openid", Constants.Scopes.GroupNames, Constants.Scopes.Groups);

                var claims = await service.GetClaimsAsync(builder.Parameters);

                Assert.AreEqual(2, claims.Count());

                var group = claims.First(p => p.Type.First() == Constants.Claims.Group).Values;
                Assert.AreEqual(1, group.Count());
                Assert.AreEqual(globalGroup.Id.ToString("N"), group.First());

                var groupNames = claims.First(p => p.Type.First() == Constants.Claims.GroupNames).Values;
                Assert.AreEqual(1, groupNames.Count());
                Assert.AreEqual(globalGroup.Name, groupNames.First());
            }
        }


        [Test]
        public async Task GetGlobalAndTenantGroupClaims_ExpectsOk()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now };
            var globalGroup = new EntityFrameworkCore.Model.Group { Id = Guid.NewGuid(), Name = "global" };
            var tenant1Group = new EntityFrameworkCore.Model.Group { Id = Guid.NewGuid(), Name = "tenant1" };
            var tenant2Group = new EntityFrameworkCore.Model.Group { Id = Guid.NewGuid(), Name = "tenant2" };

            var tenant1 = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant1" };
            var tenant2 = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant2" };

            var tenant1User = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = user.Id, TenantId = tenant1.Id };
            var tenant2User = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = user.Id, TenantId = tenant2.Id };

            var tenant1GroupAssignment = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = tenant1Group.Id, TenantId = tenant1.Id };
            var tenant2GroupAssignment = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = tenant2Group.Id, TenantId = tenant2.Id };

            var member1 = new EntityFrameworkCore.Model.RightHolderGroup { GroupId = globalGroup.Id, RightHolderId = user.Id };
            var member2 = new EntityFrameworkCore.Model.RightHolderGroup { GroupId = tenant1Group.Id, RightHolderId = user.Id };
            var member3 = new EntityFrameworkCore.Model.RightHolderGroup { GroupId = tenant2Group.Id, RightHolderId = user.Id };


            var client = new EntityFrameworkCore.Model.ClientConfiguration { Id = Guid.NewGuid(), ClientType = Model.ClientType.Native };


            using (var spScope = _serviceProvider.CreateScope())
            {

                var ctx = spScope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.AddRange(user, client, globalGroup, tenant1Group, tenant2Group, tenant1, tenant2, tenant1User, tenant2User, tenant1GroupAssignment, tenant2GroupAssignment, member1, member2, member3);
                await ctx.SaveChangesAsync();
            }

            using (var spScope = _serviceProvider.CreateScope())
            {
                var service = spScope.ServiceProvider.GetRequiredService<IClaimsService>();
                var userService = spScope.ServiceProvider.GetRequiredService<IUserService>();

                var request = new AuthorizationRequest(client.Id.ToString("N"), null, Constants.OAuth.ResponseType.Token, null, null);
                var u = await userService.GetUserByIdAsync(user.Id.ToString("N"));
                var identity = GetClaimsIdentityFromUser(u);

                IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, identity, u);
                builder
                    .WithScopes("openid", Constants.Scopes.GroupNames, Constants.Scopes.Groups, Constants.Scopes.Tenant, tenant1.Id.ToString("N"));

                var claims = await service.GetClaimsAsync(builder.Parameters);

                Assert.AreEqual(3, claims.Count());

                var group = claims.First(p => p.Type.First() == Constants.Claims.Group).Values.ToList();
                Assert.AreEqual(2, group.Count());
                Assert.Contains(globalGroup.Id.ToString("N"), group);
                Assert.Contains(tenant1Group.Id.ToString("N"), group);

                var groupNamesGlobal = claims.First(p => p.Type.First() == Constants.Claims.GroupNames && p.Type.Last() == globalGroup.Id.ToString("N")).Values;
                var groupNamesTenant = claims.First(p => p.Type.First() == Constants.Claims.GroupNames && p.Type.Last() == tenant1Group.Id.ToString("N")).Values;

                Assert.AreEqual(globalGroup.Name, groupNamesGlobal.First());
                Assert.AreEqual(tenant1Group.Name, groupNamesTenant.First());
            }
        }

        [Test]
        public async Task GetUserClaims_ExpectsOk()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now };
            var user2 = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test2", Created = DateTime.Now };

            var client = new EntityFrameworkCore.Model.ClientConfiguration { Id = Guid.NewGuid(), ClientType = Model.ClientType.Native };
            var scope = new EntityFrameworkCore.Model.Scope { Id = Guid.NewGuid(), ScopeKey = "testscope" };
            var scopeAssignment = new EntityFrameworkCore.Model.ScopeAssignment { ClientId = client.Id, ScopeId = scope.Id };
            var claimType = new EntityFrameworkCore.Model.ClaimType { Id = Guid.NewGuid(), Target = ClaimTarget.AllTokens, TypeKey = "testclaim" };
            var scopeClaim = new EntityFrameworkCore.Model.ScopeClaim { ScopeId = scope.Id, ClaimTypeId = claimType.Id };
            var claimValue = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, Value = "claimValue" };
            var claimValueU1 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, UserId = user.Id, Value = "claimValueU1" };
            var claimValueU2 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, UserId = user2.Id, Value = "claimValueU2" };

            using (var spScope = _serviceProvider.CreateScope())
            {

                var ctx = spScope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.AddRange(user, user2, client, scope, scopeAssignment, claimType, scopeClaim, claimValue, claimValueU1, claimValueU2);
                await ctx.SaveChangesAsync();
            }

            using (var spScope = _serviceProvider.CreateScope())
            {
                var service = spScope.ServiceProvider.GetRequiredService<IClaimsProvider>();
                var userService = spScope.ServiceProvider.GetRequiredService<IUserService>();

                var request = new AuthorizationRequest(client.Id.ToString("N"), null, Constants.OAuth.ResponseType.Token, null, null);
                var u = await userService.GetUserByIdAsync(user.Id.ToString("N"));
                var identity = GetClaimsIdentityFromUser(u);

                IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, identity, u);
                builder
                    .WithScopes("openid", "testscope");

                var claims = await service.GetClaimsAsync(builder.Parameters);

                Assert.AreEqual(1, claims.Count());
                var result = claims.First();
                Assert.AreEqual("testclaim", result.Type.First());

                Assert.AreEqual(2, result.Values.Count());
                Assert.Contains("claimValue", result.Values.ToList());
                Assert.Contains("claimValueU1", result.Values.ToList());
            }
        }


        [Test]
        public async Task GetTenantClaims_ExpectsOk()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now };
            var user2 = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test2", Created = DateTime.Now };
            var tenant = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant" };
            var tenant2 = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant2" };

            var tenantUser = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = user.Id, TenantId = tenant.Id };

            var client = new EntityFrameworkCore.Model.ClientConfiguration { Id = Guid.NewGuid(), ClientType = Model.ClientType.Native };
            var scope = new EntityFrameworkCore.Model.Scope { Id = Guid.NewGuid(), ScopeKey = "testscope" };
            var scopeAssignment = new EntityFrameworkCore.Model.ScopeAssignment { ClientId = client.Id, ScopeId = scope.Id };
            var claimType = new EntityFrameworkCore.Model.ClaimType { Id = Guid.NewGuid(), Target = ClaimTarget.AllTokens, TypeKey = "testclaim" };
            var scopeClaim = new EntityFrameworkCore.Model.ScopeClaim { ScopeId = scope.Id, ClaimTypeId = claimType.Id };
            var claimValue = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, Value = "claimValue" };
            var claimValueT1 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, TenantId = tenant.Id, Value = "claimValueT1" };
            var claimValueT2 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, TenantId = tenant2.Id, Value = "claimValueT2" };

            using (var spScope = _serviceProvider.CreateScope())
            {
                var ctx = spScope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.AddRange(user, user2, client, tenant, tenant2, tenantUser, scope, scopeAssignment, claimType, scopeClaim, claimValue, claimValueT1, claimValueT2);
                await ctx.SaveChangesAsync();
            }

            using (var spScope = _serviceProvider.CreateScope())
            {
                var service = spScope.ServiceProvider.GetRequiredService<IClaimsProvider>();
                var userService = spScope.ServiceProvider.GetRequiredService<IUserService>();

                var request = new AuthorizationRequest(client.Id.ToString("N"), null, Constants.OAuth.ResponseType.Token, null, null);
                var u = await userService.GetUserByIdAsync(user.Id.ToString("N"));
                var identity = GetClaimsIdentityFromUser(u);

                IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, identity, u);
                builder
                    .WithScopes("openid", "testscope", "tenant", tenant.Id.ToString("N"));

                var claims = await service.GetClaimsAsync(builder.Parameters);

                Assert.AreEqual(1, claims.Count());
                var result = claims.First();
                Assert.AreEqual("testclaim", result.Type.First());

                Assert.AreEqual(2, result.Values.Count());
                Assert.Contains("claimValue", result.Values.ToList());
                Assert.Contains("claimValueT1", result.Values.ToList());
            }
        }

        [Test]
        public async Task GetTenantUserClaims_ExpectsOk()
        {
            var user = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test", Created = DateTime.Now };
            var user2 = new EntityFrameworkCore.Model.User { Id = Guid.NewGuid(), Name = "test2", Created = DateTime.Now };
            var tenant = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant" };
            var tenant2 = new EntityFrameworkCore.Model.Tenant { Id = Guid.NewGuid(), Name = "tenant2" };

            var tenantUser = new EntityFrameworkCore.Model.TenantRightHolder { RightHolderId = user.Id, TenantId = tenant.Id };

            var client = new EntityFrameworkCore.Model.ClientConfiguration { Id = Guid.NewGuid(), ClientType = Model.ClientType.Native };
            var scope = new EntityFrameworkCore.Model.Scope { Id = Guid.NewGuid(), ScopeKey = "testscope" };
            var scopeAssignment = new EntityFrameworkCore.Model.ScopeAssignment { ClientId = client.Id, ScopeId = scope.Id };
            var claimType = new EntityFrameworkCore.Model.ClaimType { Id = Guid.NewGuid(), Target = ClaimTarget.AllTokens, TypeKey = "testclaim" };
            var scopeClaim = new EntityFrameworkCore.Model.ScopeClaim { ScopeId = scope.Id, ClaimTypeId = claimType.Id };
            var claimValue = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, Value = "claimValue" };

            var claimValueU1T1 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, UserId = user.Id, TenantId = tenant.Id, Value = "claimValueU1T1" };
            var claimValueU1T2 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, UserId = user.Id, TenantId = tenant2.Id, Value = "claimValueU1T2" };

            var claimValueT1 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, TenantId = tenant.Id, Value = "claimValueT1" };
            var claimValueU1 = new EntityFrameworkCore.Model.ClaimValue { ClaimTypeId = claimType.Id, UserId = user.Id, Value = "claimValueU1" };

            using (var spScope = _serviceProvider.CreateScope())
            {
                var ctx = spScope.ServiceProvider.GetRequiredService<CodeworxIdentityDbContext>();
                ctx.AddRange(user, user2, client, tenant, tenant2, tenantUser, scope, scopeAssignment, claimType, scopeClaim, claimValue, claimValueU1T1, claimValueU1T2, claimValueU1, claimValueT1);
                await ctx.SaveChangesAsync();
            }

            using (var spScope = _serviceProvider.CreateScope())
            {
                var service = spScope.ServiceProvider.GetRequiredService<IClaimsProvider>();
                var userService = spScope.ServiceProvider.GetRequiredService<IUserService>();

                var request = new AuthorizationRequest(client.Id.ToString("N"), null, Constants.OAuth.ResponseType.Token, null, null);
                var u = await userService.GetUserByIdAsync(user.Id.ToString("N"));
                var identity = GetClaimsIdentityFromUser(u);

                IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, identity, u);
                builder
                    .WithScopes("openid", "testscope", "tenant", tenant.Id.ToString("N"));

                var claims = await service.GetClaimsAsync(builder.Parameters);

                Assert.AreEqual(1, claims.Count());
                var result = claims.First();
                Assert.AreEqual("testclaim", result.Type.First());

                Assert.AreEqual(4, result.Values.Count());
                Assert.Contains("claimValue", result.Values.ToList());
                Assert.Contains("claimValueU1", result.Values.ToList());
                Assert.Contains("claimValueT1", result.Values.ToList());
                Assert.Contains("claimValueU1T1", result.Values.ToList());
            }
        }

        private ClaimsIdentity GetClaimsIdentityFromUser(IUser user)
        {
            var identity = new ClaimsIdentity(Constants.DefaultAuthenticationScheme);

            identity.AddClaim(new Claim(Constants.Claims.Id, user.Identity));
            identity.AddClaim(new Claim(Constants.Claims.Upn, user.Name));
            identity.AddClaim(new Claim(Constants.Claims.Session, Guid.NewGuid().ToString("N")));

            if (user.ForceChangePassword)
            {
                identity.AddClaim(new Claim(Constants.Claims.ForceChangePassword, "true"));
            }

            if (user.AuthenticationMode == AuthenticationMode.Mfa)
            {
                identity.AddClaim(new Claim(Constants.Claims.ForceMfaLogin, "true"));
            }

            if (user.ConfirmationPending)
            {
                identity.AddClaim(new Claim(Constants.Claims.ConfirmationPending, "true"));
            }

            if (!string.IsNullOrWhiteSpace(user.DefaultTenantKey))
            {
                identity.AddClaim(new Claim(Constants.Claims.DefaultTenant, user.DefaultTenantKey));
            }

            return identity;
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