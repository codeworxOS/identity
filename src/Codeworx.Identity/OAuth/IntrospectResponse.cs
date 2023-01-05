using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Codeworx.Identity.Model;
using Newtonsoft.Json;

namespace Codeworx.Identity.OAuth
{
    [DataContract]
    public class IntrospectResponse : IIntrospectResponse
    {
        public IntrospectResponse(bool isActive)
        {
            IsActive = isActive;
            AdditionalProperties = new Dictionary<string, object>();
        }

        public IntrospectResponse(DateTime validUntil, System.Collections.Generic.IDictionary<string, object> dictionary)
            : this(true)
        {
            AdditionalProperties = dictionary;
            Expiration = ((DateTimeOffset)validUntil).ToUnixTimeSeconds();
        }

        [JsonProperty(PropertyName = "active")]
        public bool IsActive { get; }

        [JsonProperty(PropertyName = "exp", NullValueHandling = NullValueHandling.Ignore)]
        public long? Expiration { get; }

        [JsonExtensionData]
        public IDictionary<string, object> AdditionalProperties { get; }
    }
}
