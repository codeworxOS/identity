using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Cryptography.Argon2;
using Codeworx.Identity.Cryptography.Pbkdf2;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityCryptographyIdentityBuilderExtensions
    {
        public static IIdentityServiceBuilder Pbkdf2(this IIdentityServiceBuilder builder, KeyDerivationPrf algorithm = KeyDerivationPrf.HMACSHA256, int iterations = 10000, byte saltLength = 32, byte outputLength = 32)
        {
            return builder.ReplaceService<IHashingProvider, Pbkdf2HashingProvider>(ServiceLifetime.Singleton)
                        .ReplaceService<Pbkdf2Options, Pbkdf2Options>(ServiceLifetime.Singleton, sp => new Pbkdf2Options { HashAlgorithm = algorithm, Iterations = iterations, SaltLength = saltLength, OutputLength = outputLength });
        }

        public static IIdentityServiceBuilder Argon2(this IIdentityServiceBuilder builder, Argon2Options options = null)
        {
            return builder.ReplaceService<IHashingProvider, Argon2HashingProvider>(ServiceLifetime.Singleton)
                        .ReplaceService<Argon2Options, Argon2Options>(ServiceLifetime.Singleton, sp => options ?? new Argon2Options());
        }

        public static IIdentityServiceBuilder WithAesSymmetricEncryption(this IIdentityServiceBuilder builder, Argon2Options options = null)
        {
            return builder.ReplaceService<ISymmetricDataEncryption, AesDataEncryption>(ServiceLifetime.Singleton);
        }
    }
}