using System.Security.Cryptography.X509Certificates;

namespace Codeworx.Identity.Configuration
{
    public class SigningOptions
    {
        public SigningOptions()
        {
            this.Location = StoreLocation.CurrentUser;
            this.Name = nameof(StoreName.My);
            this.FindBy = X509FindType.FindBySubjectDistinguishedName;
            this.Source = KeySource.TemporaryInMemory;
        }

        public SigningOptions(SigningOptions signing)
        {
            this.Location = signing.Location;
            this.Name = signing.Name;
            this.FindBy = signing.FindBy;
            this.Source = signing.Source;
            this.Search = signing.Search;
        }

        public KeySource Source { get; set; }

        public StoreLocation Location { get; set; }

        public string Name { get; set; }

        public X509FindType FindBy { get; set; }

        public string Search { get; set; }
    }
}
