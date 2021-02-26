using System;
using System.Threading.Tasks;
using Codeworx.Identity.Invitation;

namespace Codeworx.Identity.Cache
{
    public interface IInvitationCache
    {
        Task<InvitationItem> GetAsync(string code);

        Task AddAsync(string code, InvitationItem factory, TimeSpan validity);

        Task UpdateAsync(string code, Action<InvitationItem> update);
    }
}
