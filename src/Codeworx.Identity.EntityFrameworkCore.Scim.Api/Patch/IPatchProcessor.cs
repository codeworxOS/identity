using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Patch
{
    public interface IPatchProcessor
    {
        Task<TOutput> ProcessAsync<TInput, TOutput>(TInput resource, PatchResource patch);
    }
}
