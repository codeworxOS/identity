using System.Collections;
using System.Collections.Generic;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    public class ListResponse<T>
    {
        public ListResponse(int startIndex, int totalResults, int itemsPerPage, List<T> resources)
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

        public ListResponse(List<T> resources)
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

        public List<T> Resources { get; }
    }
}