namespace Codeworx.Identity.Response
{
    public class NotAcceptableResponse
    {
        public NotAcceptableResponse(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }
}
