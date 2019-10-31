using System.Collections.Generic;
using System.Collections.ObjectModel;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class SupportedFlows
    {
        public static IReadOnlyList<ISupportedFlow> GetFlow(FlowType flowTypes)
        {
            var list = new List<ISupportedFlow>();

            if (flowTypes.HasFlag(FlowType.AuthorizationCode) || flowTypes.HasFlag(FlowType.Code))
            {
                list.Add(new AuthorizationCodeSupportedFlow());
            }

            if (flowTypes.HasFlag(FlowType.Token))
            {
                list.Add(new TokenSupportedFlow());
            }

            return new ReadOnlyCollection<ISupportedFlow>(list);
        }

        private class AuthorizationCodeSupportedFlow : ISupportedFlow
        {
            public bool IsSupported(string flowKey)
            {
                return flowKey == OAuth.Constants.ResponseType.Code || flowKey == OAuth.Constants.GrantType.AuthorizationCode;
            }
        }

        private class TokenSupportedFlow : ISupportedFlow
        {
            public bool IsSupported(string flowKey)
            {
                return flowKey == OAuth.Constants.ResponseType.Token;
            }
        }
    }
}