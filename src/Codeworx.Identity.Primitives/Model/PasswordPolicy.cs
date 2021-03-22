namespace Codeworx.Identity.Model
{
    public class PasswordPolicy
    {
        public PasswordPolicy(string regex, string description)
        {
            this.Regex = regex;
            this.Description = description;
        }

        public string Regex { get; }

        public string Description { get; }
    }
}
