using System.Collections.Generic;
using System.Linq;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model
{
    public class ListResponse<T>
    {
        public ListResponse(int startIndex, int totalResults, int itemsPerPage, IEnumerable<T> resources)
        {
            Schemas = new string[]
            {
                SchemaConstants.List,
            };

            StartIndex = startIndex;
            TotalResults = totalResults;
            ItemsPerPage = itemsPerPage;

            Resources = resources.ToList();
        }

        public ListResponse(IEnumerable<T> resources)
        {
            Schemas = new string[]
            {
                SchemaConstants.List,
            };

            Resources = resources.ToList();

            StartIndex = 1;
            TotalResults = Resources.Count;
            ItemsPerPage = Resources.Count;
        }

        public string[] Schemas { get; set; }

        public int ItemsPerPage { get; set; }

        public int StartIndex { get; set; }

        public int TotalResults { get; set; }

        public ICollection<T> Resources { get; }
    }
}
