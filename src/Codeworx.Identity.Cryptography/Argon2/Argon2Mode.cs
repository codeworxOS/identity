using System;

namespace Codeworx.Identity.Cryptography.Argon2
{
    [Flags]
    public enum Argon2Mode
    {
        None = 0x00,
        Argon2i = 0x01,
        Argon2d = 0x02,
        Argon2id = Argon2i | Argon2d,
    }
}