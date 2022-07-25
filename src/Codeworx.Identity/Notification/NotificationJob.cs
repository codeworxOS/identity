using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.Notification
{
    public class NotificationJob : BackgroundService
    {
        // EventIds 151xx
        private static readonly Action<ILogger, Exception> _logNotificationJobStarted;
        private static readonly Action<ILogger, Exception> _logNotificationJobStopped;
        private static readonly Action<ILogger, Exception> _logNotificationProcessorError;
        private static readonly Action<ILogger, Exception> _logNotificationRunError;
        private readonly INotificationQueue _queue;
        private readonly ILogger<NotificationJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        static NotificationJob()
        {
            _logNotificationJobStarted = LoggerMessage.Define(LogLevel.Information, new EventId(15101), "Notification job started.");
            _logNotificationJobStopped = LoggerMessage.Define(LogLevel.Information, new EventId(15102), "Notification job stopped.");
            _logNotificationProcessorError = LoggerMessage.Define(LogLevel.Error, new EventId(15103), "An error occured when processing a notification.");
            _logNotificationRunError = LoggerMessage.Define(LogLevel.Error, new EventId(15104), "Unhandled Error running notification job.");
        }

        public NotificationJob(INotificationQueue queue, ILogger<NotificationJob> logger, IServiceProvider serviceProvider)
        {
            _queue = queue;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logNotificationJobStarted(_logger, null);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var item = await _queue.DequeueAsync(stoppingToken).ConfigureAwait(false);
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        try
                        {
                            var processor = scope.ServiceProvider.GetRequiredService<INotificationProcessor>();
                            await processor.SendNotificationAsync(item).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logNotificationProcessorError(_logger, ex);
                        }
                    }
                }

                _logNotificationJobStopped(_logger, null);
            }
            catch (OperationCanceledException)
            {
                _logNotificationJobStopped(_logger, null);
            }
            catch (Exception ex)
            {
                _logNotificationRunError(_logger, ex);
                throw;
            }
        }
    }
}
