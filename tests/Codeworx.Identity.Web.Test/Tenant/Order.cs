using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.Web.Test.Tenant
{
    public class Order
    {
        public Guid Id { get; set; }

        [StringLength(10)]
        [Required]
        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        [StringLength(2000)]
        public string OrderDescription { get; set; }
    }
}