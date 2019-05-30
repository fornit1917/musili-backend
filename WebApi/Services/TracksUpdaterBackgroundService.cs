using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Musili.WebApi.Interfaces;
using NLog;

namespace Musili.WebApi.Services {
    public class TracksUpdaterBackgroundService : IHostedService, IDisposable {
        private IServiceProvider _services;
        private ILogger<TracksUpdaterBackgroundService> _logger;
        private Timer _timer;
        private bool _started = false;
        private AppConfig _appConfig;

        public TracksUpdaterBackgroundService(IServiceProvider services, ILogger<TracksUpdaterBackgroundService> logger, AppConfig appConfig) {
            _services = services;
            _logger = logger;
            _appConfig = appConfig;
        }

        public Task StartAsync(CancellationToken cancellationToken) {
            if (_appConfig.DeleteOldTracksInBackground || _appConfig.LoadNewTracksInBackground) {
                _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(_appConfig.TracksUpdaterTimeoutSeconds));
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void DoWorkAsync(object state) {
            if (_started) {
                return;
            }

            _started = true;
            using (var scope = _services.CreateScope()) {
                ITracksUpdater tracksUpdater = scope.ServiceProvider.GetRequiredService<ITracksUpdater>();
                try {
                    MappedDiagnosticsLogicalContext.Set("backgroundTaskId", Guid.NewGuid());
                    _logger.LogTrace("Background taks has been started");

                    if (_appConfig.DeleteOldTracksInBackground) {
                        await tracksUpdater.RemoveOldTracksAsync();
                    }
                    if (_appConfig.LoadNewTracksInBackground) {
                        await tracksUpdater.LoadNewTracksForAllCriteriasAsync(_appConfig.TracksUpdaterMaxDurationSeconds);
                    }

                    _logger.LogTrace("Background taks has been completed successfully");
                } catch (Exception e) {
                    _logger.LogError(e, "Unhandled exception in background tracks updater background task");
                } finally {
                    MappedDiagnosticsContext.Remove("backgroundTaskId");
                    _started = false;
                }
            }
        }

        public void Dispose() {
            _timer?.Dispose();
        }
    }
}