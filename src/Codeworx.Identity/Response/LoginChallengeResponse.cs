namespace Codeworx.Identity.Response
{
    public class LoginChallengeResponse
    {
        public LoginChallengeResponse(string prompt = null)
        {
            Prompt = prompt;
        }

        public string Prompt { get; }
    }
}
