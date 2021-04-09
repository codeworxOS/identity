using System.Collections.Generic;

namespace Codeworx.Identity.Wpf.Test.Common
{
    public interface ISessionInfo
    {
        string AccessToken { get; }

        IDictionary<string, object> Claims { get; }
    }
}
