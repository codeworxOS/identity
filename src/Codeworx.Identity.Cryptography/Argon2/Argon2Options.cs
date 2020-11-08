namespace Codeworx.Identity.Cryptography.Argon2
{
    public class Argon2Options
    {
        public Argon2Options()
        {
            DegreeOfParallelism = 8;
            MemorySize = 8192;
            Iterations = 2;
            Argon2Mode = Argon2Mode.Argon2i;
            HashLength = 32;
            SaltLength = 32;
        }

        public int DegreeOfParallelism { get; set; }

        public int MemorySize { get; set; }

        public int Iterations { get; set; }

        public Argon2Mode Argon2Mode { get; set; }

        public int HashLength { get; set; }

        public int SaltLength { get; set; }
    }
}
