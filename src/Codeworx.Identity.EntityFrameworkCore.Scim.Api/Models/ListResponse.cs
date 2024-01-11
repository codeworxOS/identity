using System.Collections;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models
{
    public class ListResponse
    {
        public ListResponse(int startIndex, int totalResults, int itemsPerPage, IList resources)
        {
            Schemas = new string[]
            {
                ScimConstants.Schemas.List,
            };

            StartIndex = startIndex;
            TotalResults = totalResults;
            ItemsPerPage = itemsPerPage;

            Resources = resources;
        }

        public ListResponse(IList resources)
        {
            Schemas = new string[]
            {
                ScimConstants.Schemas.List,
            };

            Resources = resources;

            StartIndex = 1;
            TotalResults = Resources.Count;
            ItemsPerPage = Resources.Count;
        }

        public string[] Schemas { get; set; }

        public int ItemsPerPage { get; set; }

        public int StartIndex { get; set; }

        public int TotalResults { get; set; }

        [JsonPropertyName("Resources")]
        public IList Resources { get; }
    }
}