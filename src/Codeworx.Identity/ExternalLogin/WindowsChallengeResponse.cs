namespace Codeworx.Identity.ExternalLogin
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
