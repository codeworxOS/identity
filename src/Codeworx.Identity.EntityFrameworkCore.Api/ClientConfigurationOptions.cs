namespace Codeworx.Identity.EntityFrameworkCore.Api
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
