namespace Codeworx.Identity.Configuration
{
    public class MaxLengthOption
    {
        public MaxLengthOption()
        {
        }

        public MaxLengthOption(MaxLengthOption maxLength)
        {
            Login = maxLength.Login;
            Password = maxLength.Password;
        }

        public int Password { get; set; }

        public int Login { get; set; }
    }
}