using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class ErrorResource
    {
        public ErrorResource(ScimType scimType, int statusCode, string? detail = null)
        {
            Status = $"{statusCode}";

            switch (scimType)
            {
                case Error.ScimType.InvalidFilter:
                    ScimType = ScimConstants.Error.InvalidFilter;
                    break;
                case Error.ScimType.TooMany:
                    ScimType = ScimConstants.Error.TooMany;
                    break;
                case Error.ScimType.Uniqueness:
                    ScimType = ScimConstants.Error.Uniqueness;
                    break;
                case Error.ScimType.Mutability:
                    ScimType = ScimConstants.Error.Mutability;
                    break;
                case Error.ScimType.InvalidSyntax:
                    ScimType = ScimConstants.Error.InvalidSyntax;
                    break;
                case Error.ScimType.InvalidPath:
                    ScimType = ScimConstants.Error.InvalidPath;
                    break;
                case Error.ScimType.NoTarget:
                    ScimType = ScimConstants.Error.NoTarget;
                    break;
                case Error.ScimType.InvalidValue:
                    ScimType = ScimConstants.Error.InvalidValue;
                    break;
                case Error.ScimType.InvalidVers:
                    ScimType = ScimConstants.Error.InvalidVers;
                    break;
                case Error.ScimType.Sensitive:
                    ScimType = ScimConstants.Error.Sensitive;
                    break;
                default:
                    ScimType = ScimConstants.Error.InvalidSyntax;
                    break;
            }

            Detail = detail;
        }

        public string[] Schemas { get; } = new[] { ScimConstants.Schemas.Error };

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("scimType")]
        public string ScimType { get; set; }

        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }
}
