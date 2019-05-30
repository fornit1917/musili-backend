using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Musili.WebApi.Interfaces;
using Musili.WebApi.Models;
using NLog;

namespace Musili.WebApi.Services.Grabbers {
    public class TracksGrabber : ICommonTracksGrabber {
        private Func<TracksSourceService, ITracksGrabber> _grabbersProvider;
        private ISemaphores _semaphores;

        public TracksGrabber(Func<TracksSourceService, ITracksGrabber> grabbersProvider, ISemaphores semaphores) {
            _grabbersProvider = grabbersProvider;
            _semaphores = semaphores;
        }

        public async Task<List<Track>> GrabRandomTracksAsync(TracksSource tracksSource) {
            SemaphoreSlim semaphore = _semaphores.GetSemaphoreForService(tracksSource.Service);
            await semaphore.WaitAsync();
            try {
                MappedDiagnosticsLogicalContext.Set("grabRequestId", Guid.NewGuid());
                ITracksGrabber grabber = _grabbersProvider(tracksSource.Service);
                DateTime expirationDatetime = DateTime.Now.Add(grabber.LinkLifeTime);
                List<Track> tracks = await grabber.GrabRandomTracksAsync(tracksSource);
                foreach (var track in tracks) {
                    track.TracksSource = tracksSource;
                    track.ExpirationDatetime = expirationDatetime;
                }
                return tracks;
            } finally {
                MappedDiagnosticsLogicalContext.Remove("grabRequestId");
                semaphore.Release();
            }
        }
    }
}