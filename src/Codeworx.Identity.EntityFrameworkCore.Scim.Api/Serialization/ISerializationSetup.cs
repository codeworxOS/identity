using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Serialization
{
    public interface ISerializationSetup
    {
        JsonOptions GetJsonFormatterOptions();

        JsonSerializerOptions GetOptionsForDeserialize();

        JsonSerializerOptions GetOptionsForSerialize();
    }
}
