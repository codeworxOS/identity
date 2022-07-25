using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Notification
{
    public class NotificationMemoryQueue : INotificationQueue, IDisposable
    {
        private readonly BlockingCollection<INotification> _queue;
        private bool _disposedValue;

        public NotificationMemoryQueue()
        {
            _queue = new BlockingCollection<INotification>(200);
        }

        public Task<INotification> DequeueAsync(CancellationToken token = default)
        {
            return Task.Run<INotification>(
                () =>
                {
                    while (!_queue.IsCompleted)
                    {
                        if (_queue.TryTake(out var notification, 1000, token))
                        {
                            return notification;
                        }
                    }

                    throw new NotSupportedException();
                }, token);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task EnqueueAsync(INotification notification, CancellationToken token = default)
        {
            if (!_queue.TryAdd(notification, 1000, token))
            {
                throw new NotSupportedException();
            }

            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _queue.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
