using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error
{
    public class ScimException : Exception
    {
        public ScimException(ScimType scimType, int statusCode = 400)
            : base($"Scim error {scimType} occured with status [{statusCode}]")
        {
            ScimType = scimType;
            StatusCode = statusCode;
        }

        public ScimType ScimType { get; }

        public int StatusCode { get; }
    }
}
