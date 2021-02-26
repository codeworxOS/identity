namespace Codeworx.Identity.Model
{
    public class InvitationViewRequest
    {
        public InvitationViewRequest(string code)
        {
            Code = code;
        }

        public string Code { get; }
    }
}
