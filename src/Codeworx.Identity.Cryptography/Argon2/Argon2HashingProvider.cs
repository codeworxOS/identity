using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;

namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2HashingProvider : IHashingProvider
    {
        private const int PARALLELISM = 2;
        private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        private readonly Argon2Options _options;

        public Argon2HashingProvider(Argon2Options options)
        {
            _options = options;
        }

        public string Create(string plaintext)
        {
            byte[] salt = CreateSalt(16);

            var password = Encoding.UTF8.GetBytes(plaintext);

            var argon = GetArgon2Config(_options.Argon2Mode);
            argon.Version = Argon2Version.Nineteen;
            argon.TimeCost = _options.Iterations;
            argon.MemoryCost = _options.MemorySize;
            argon.Lanes = PARALLELISM;
            argon.Threads = PARALLELISM;
            argon.Password = password;
            argon.Salt = salt;
            argon.HashLength = _options.HashLength;

            var hasher = new Isopoh.Cryptography.Argon2.Argon2(argon);

            using (var hash = hasher.Hash())
            {
                var encodedSalt = Convert.ToBase64String(salt).TrimEnd('=');
                var encodedHash = Convert.ToBase64String(hash.Buffer).TrimEnd('=');

                var result = $"${_options.Argon2Mode.ToString().ToLower()}$v=19$m={_options.MemorySize},t={_options.Iterations},p={PARALLELISM}${encodedSalt}${encodedHash}";
                return result;
            }
        }

        public bool Validate(string plaintext, string hashValue)
        {
            var password = Encoding.UTF8.GetBytes(plaintext);

            var input = hashValue.Split('$');

            if (input.Length != 6)
            {
                throw new ArgumentException("Invalid format!", nameof(hashValue));
            }

            if (input[2] != "v=19")
            {
                throw new ArgumentException("Unsupported Argon2 Version", nameof(hashValue));
            }

            var mode = input[1];
            var parameters = input[3].Split(',').Select(p => p.Split('=')).ToDictionary(p => p[0], p => int.Parse(p[1]));

            int memorySize;
            int iterations;
            int degreeOfParallelism;

            if (!parameters.TryGetValue("m", out memorySize))
            {
                memorySize = _options.MemorySize;
            }

            if (!parameters.TryGetValue("t", out iterations))
            {
                iterations = _options.Iterations;
            }

            if (!parameters.TryGetValue("p", out degreeOfParallelism))
            {
                degreeOfParallelism = PARALLELISM;
            }

            var salt = DecodeBase64(input[4]);
            var hash = DecodeBase64(input[5]);

            var argon = GetArgon2Config(mode);
            argon.Version = Argon2Version.Nineteen;
            argon.TimeCost = iterations;
            argon.MemoryCost = memorySize;
            argon.Lanes = degreeOfParallelism;
            argon.Threads = degreeOfParallelism;
            argon.Password = password;
            argon.Salt = salt;
            argon.HashLength = hash.Length;

            var hasher = new Isopoh.Cryptography.Argon2.Argon2(argon);

            using (var compareHash = hasher.Hash())
            {
                return compareHash.Buffer.SequenceEqual(hash);
            }
        }

        private static byte[] CreateSalt(int size)
        {
            byte[] data = new byte[4 * size];
            using (var random = RandomNumberGenerator.Create())
            {
                random.GetBytes(data);
            }

            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % _chars.Length;

                result.Append(_chars[idx]);
            }

            return Encoding.UTF8.GetBytes(result.ToString());
        }

        private static byte[] DecodeBase64(string encoded)
        {
            var add = encoded.Length % 4;
            string padding = add > 0 ? new string('=', 4 - add) : string.Empty;

            return Convert.FromBase64String(encoded + padding);
        }

        private Argon2Config GetArgon2Config(string mode)
        {
            if (Enum.TryParse<Argon2Mode>(mode, true, out var modeEnum))
            {
                return GetArgon2Config(modeEnum);
            }

            throw new NotSupportedException($"Mode {mode} not supported!");
        }

        private Argon2Config GetArgon2Config(Argon2Mode mode)
        {
            switch (mode)
            {
                case Argon2Mode.Argon2i:
                    return new Argon2Config { Type = Argon2Type.DataIndependentAddressing };
                case Argon2Mode.Argon2d:
                    return new Argon2Config { Type = Argon2Type.DataDependentAddressing };
                case Argon2Mode.Argon2id:
                    return new Argon2Config { Type = Argon2Type.HybridAddressing };
                case Argon2Mode.None:
                default:
                    throw new NotSupportedException();
            }

            throw new NotSupportedException($"Mode {mode} not supported!");
        }
    }
}
