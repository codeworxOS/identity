using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cache
{
    public class InvitationAlreadyRedeemedException : Exception
    {
        public InvitationAlreadyRedeemedException()
            : this("Invitation aready redeemed!")
        {
        }

        protected InvitationAlreadyRedeemedException(string message)
            : base(message)
        {
        }

        protected InvitationAlreadyRedeemedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvitationAlreadyRedeemedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
