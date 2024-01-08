namespace Codeworx.Identity.Cryptography.Argon2
{
    public class ClientConfigurationOptions
    {
        public ClientConfigurationOptions()
        {
            SecretLength = 32;
        }

        public int SecretLength { get; set; }
    }
}
