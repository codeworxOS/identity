namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2Options
    {
        public Argon2Options()
        {
            MemorySize = 65535;
            HashLength = 32;
            Iterations = 5;
            Argon2Mode = Argon2Mode.Argon2i;
        }

        public int MemorySize { get; set; }

        public int Iterations { get; set; }

        public Argon2Mode Argon2Mode { get; set; }

        public int HashLength { get; set; }
    }
}
