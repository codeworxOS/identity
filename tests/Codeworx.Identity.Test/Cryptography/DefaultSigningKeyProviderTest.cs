using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Codeworx.Identity.Test.Cryptography
{
    public class DefaultSigningKeyProviderTest
    {
        private ServiceProvider _serviceProvider;
        private string _thumbprint;

        [SetUp]
        public void SetUp()
        {
            var name = Guid.NewGuid().ToString("N");
            X500DistinguishedName distinguishedName = new X500DistinguishedName($"CN={name}");
            using (RSA rsa = RSA.Create(2048))
            {
                var request = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                ////request.CertificateExtensions.Add(
                ////    new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));


                ////request.CertificateExtensions.Add(
                ////   new X509EnhancedKeyUsageExtension(
                ////       new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

                var certificate = request.CreateSelfSigned(new DateTimeOffset(DateTime.UtcNow.AddDays(-1)), new DateTimeOffset(DateTime.UtcNow.AddDays(10)));
                certificate.FriendlyName = name;

                using (var cert = new X509Certificate2(certificate.Export(X509ContentType.Pfx, "WeNeedASaf3rPassword"), "WeNeedASaf3rPassword", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet))
                {
                    _thumbprint = cert.Thumbprint;
                    using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                    {
                        store.Open(OpenFlags.ReadWrite);
                        store.Add(cert);
                        store.Close();
                    }
                }
            }

            var services = new ServiceCollection();
            services.Configure<IdentityOptions>(p =>
            {
                p.Signing.Source = KeySource.Store;
                p.Signing.Location = System.Security.Cryptography.X509Certificates.StoreLocation.CurrentUser;
                p.Signing.Search = _thumbprint;
                p.Signing.FindBy = System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint;
            });
            services.AddCodeworxIdentity();

            _serviceProvider = services.BuildServiceProvider();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadWrite);
                var cert = store.Certificates.Find(X509FindType.FindByThumbprint, _thumbprint, false);
                if (cert.Any())
                {
                    store.Remove(cert.First());
                }

                store.Close();

            }

            await _serviceProvider.DisposeAsync();
        }

        public DefaultSigningKeyProviderTest()
        {

        }

        [Test]
        public async Task TestRsajwkInformationSerializerAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dataProvider = scope.ServiceProvider.GetRequiredService<IDefaultSigningDataProvider>();
            var jwkInformationSerializer = scope.ServiceProvider.GetServices<IJwkInformationSerializer>();

            var data = await dataProvider.GetSigningDataAsync(default).ConfigureAwait(false);

            var defaultKey = data.Key;
            var hashAlgorithm = data.Hash;
            var serializer = jwkInformationSerializer.First(p => p.Supports(defaultKey));

            var supportedSigningAlgorithms = new[] { serializer.GetAlgorithm(defaultKey, hashAlgorithm) };

            Assert.Contains("RS256", supportedSigningAlgorithms);
        }
    }
}
