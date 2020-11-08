using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2HashingProvider : IHashingProvider
    {
        private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        private readonly Argon2Options _options;

        public Argon2HashingProvider(Argon2Options options)
        {
            _options = options;
        }

        public string Create(string plaintext)
        {
            byte[] salt = CreateSalt(_options.SaltLength);

            var password = Encoding.UTF8.GetBytes(plaintext);

            var argon = GetArgon2Implementation(_options.Argon2Mode, password);
            argon.Iterations = _options.Iterations;
            argon.MemorySize = _options.MemorySize;
            argon.DegreeOfParallelism = _options.DegreeOfParallelism;
            argon.Salt = salt;

            var hash = argon.GetBytes(_options.HashLength);

            var encodedSalt = Convert.ToBase64String(salt).TrimEnd('=');
            var encodedHash = Convert.ToBase64String(hash).TrimEnd('=');

            var result = $"${_options.Argon2Mode.ToString().ToLower()}$v=19$m={_options.MemorySize},t={_options.Iterations},p={_options.DegreeOfParallelism}${encodedSalt}${encodedHash}";

            return result;
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
                degreeOfParallelism = _options.DegreeOfParallelism;
            }

            var salt = DecodeBase64(input[4]);
            var hash = DecodeBase64(input[5]);

            var argon = GetArgon2Implementation(mode, password);
            argon.Iterations = iterations;
            argon.MemorySize = memorySize;
            argon.DegreeOfParallelism = degreeOfParallelism;
            argon.Salt = salt;

            var compareHash = argon.GetBytes(hash.Length);

            return compareHash.SequenceEqual(hash);
        }

        private static byte[] DecodeBase64(string encoded)
        {
            var add = encoded.Length % 4;
            string padding = add > 0 ? new string('=', 4 - add) : string.Empty;

            return Convert.FromBase64String(encoded + padding);
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

        private Konscious.Security.Cryptography.Argon2 GetArgon2Implementation(string mode, byte[] password)
        {
            if (Enum.TryParse<Argon2Mode>(mode, true, out var modeEnum))
            {
                return GetArgon2Implementation(modeEnum, password);
            }

            throw new NotSupportedException($"Mode {mode} not supported!");
        }

        private Konscious.Security.Cryptography.Argon2 GetArgon2Implementation(Argon2Mode mode, byte[] password)
        {
            switch (mode)
            {
                case Argon2Mode.Argon2i:
                    return new Konscious.Security.Cryptography.Argon2i(password);
                case Argon2Mode.Argon2d:
                    return new Konscious.Security.Cryptography.Argon2d(password);
                case Argon2Mode.Argon2id:
                    return new Konscious.Security.Cryptography.Argon2id(password);
            }

            throw new NotSupportedException($"Mode {mode} not supported!");
        }
    }
}
