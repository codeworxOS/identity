#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1310 // Field names should not contain underscore
#pragma warning disable IDE1006 // Naming Styles
using System;
using System.Runtime.InteropServices;

namespace Codeworx.Identity.Cryptography.Interop
{
    internal static class Libsodium
    {
        internal const uint ARGON_HASH_STR_SIZE = 128U;

        internal const int crypto_pwhash_argon2i_ALG_ARGON2I13 = 1;
        internal const int crypto_pwhash_argon2id_ALG_ARGON2ID13 = 2;

        private const string Library = "libsodium";

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_pwhash(
            byte[] buffer,
            ulong outlen,
            byte[] passwd,
            ulong passwdlen,
            byte[] salt,
            ulong opslimit,
            uint memlimit,
            int alg);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int crypto_pwhash_str_alg(
            byte[] hash,
            byte[] passwd,
            ulong passwdlen,
            ulong opslimit,
            uint memlimit,
            int alg);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]

        internal static extern int crypto_pwhash_str_verify(
            byte[] hash,
            byte[] passwd,
            ulong passwdlen);

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sodium_init();

        [DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int sodium_set_misuse_handler(
            Action handler);
    }
}

#pragma warning restore SA1300 // Element should begin with upper-case letter
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1310 // Field names should not contain underscore
#pragma warning restore SA1303 // Const field names should begin with upper-case letter