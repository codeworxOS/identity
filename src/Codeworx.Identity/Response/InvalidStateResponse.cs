namespace Codeworx.Identity.Response
{
    public class InvalidStateResponse
    {
        public InvalidStateResponse(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; }
    }
}