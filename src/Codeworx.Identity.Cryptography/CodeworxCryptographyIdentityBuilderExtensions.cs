using Codeworx.Identity.Configuration;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Cryptography
{
    public static class CodeworxCryptographyIdentityBuilderExtensions
    {
        public static IIdentityServiceBuilder Pbkdf2(this IIdentityServiceBuilder builder, KeyDerivationPrf algorithm = KeyDerivationPrf.HMACSHA256, int iterations = 10000, byte saltLength = 32, byte outputLength = 32)
        {
            return builder.ReplaceService<IHashingProvider, Pbkdf2HashingProvider>(ServiceLifetime.Singleton)
                        .ReplaceService<Pbkdf2Options, Pbkdf2Options>(ServiceLifetime.Singleton, sp => new Pbkdf2Options { HashAlgorithm = algorithm, Iterations = iterations, SaltLength = saltLength, OutputLength = outputLength });
        }
    }
}