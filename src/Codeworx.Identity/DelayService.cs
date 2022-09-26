using System;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public class DelayService : ILoginDelayService, IForgotPasswordDelayService
    {
        private TimeSpan _delay;
        private int _count;
        private object _locker = new object();

        public DelayService()
        {
            _delay = TimeSpan.FromMilliseconds(40);
            _count = 0;
        }

        public async Task DelayAsync()
        {
            await Task.Delay(_delay);
        }

        public void Record(TimeSpan duration)
        {
            TimeSpan newValue = TimeSpan.Zero;
            lock (_locker)
            {
                if (_count == int.MaxValue)
                {
                    _count = 1;
                }

                var current = TimeSpan.FromTicks(_count * _delay.Ticks);
                _count++;
                newValue = TimeSpan.FromTicks((current + duration).Ticks / _count);
            }

            _delay = newValue;
        }
    }
}
