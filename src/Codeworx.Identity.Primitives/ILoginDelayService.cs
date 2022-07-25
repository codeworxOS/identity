using System;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface ILoginDelayService
    {
        Task DelayAsync();

        void Record(TimeSpan duration);
    }
}
