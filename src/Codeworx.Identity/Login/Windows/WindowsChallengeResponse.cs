namespace Codeworx.Identity.Login.Windows
{
    public class WindowsChallengeResponse
    {
        public WindowsChallengeResponse(bool doChallenge = true)
        {
            DoChallenge = doChallenge;
        }

        public bool DoChallenge { get; }
    }
}
