using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class IPv4ProviderFilter : ProviderFilter
    {
        [Required]
        public byte[] RangeEnd { get; set; }

        [Required]
        public byte[] RangeStart { get; set; }
    }
}