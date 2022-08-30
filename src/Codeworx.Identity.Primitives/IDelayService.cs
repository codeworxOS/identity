using System;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IDelayService
    {
        Task DelayAsync();

        void Record(TimeSpan duration);
    }
}
