using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Binding
{
    internal class RequestResourceBinderAttribute : ModelBinderAttribute
    {
        public RequestResourceBinderAttribute()
            : base(typeof(RequestResourceBinding))
        {
            BindingSource = BindingSource.Body;
        }
    }
}