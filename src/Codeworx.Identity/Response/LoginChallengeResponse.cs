namespace Codeworx.Identity.Response
{
    public class LoginChallengeResponse
    {
        public LoginChallengeResponse(string prompt)
        {
            Prompt = prompt;
        }

        public string Prompt { get; }
    }
}
