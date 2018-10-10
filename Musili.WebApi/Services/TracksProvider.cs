﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Musili.WebApi.Interfaces;
using Musili.WebApi.Models;

namespace Musili.WebApi.Services
{
    public class TracksProvider : ITracksProvider
    {
        private ITracksSourcesRepository tracksSourceRepository;
        private ITracksRepository tracksRepository;
        private ICommonTracksGrabber grabber;

        public TracksProvider(ITracksSourcesRepository tracksSourceRepository, ITracksRepository tracksRepository, ICommonTracksGrabber grabber) {
            this.tracksSourceRepository = tracksSourceRepository;
            this.tracksRepository = tracksRepository;
            this.grabber = grabber;
        }

        public async Task<List<Track>> GetTracksAsync(TracksCriteria criteria, int lastId = 0) {
            TracksSource tracksSource =  await tracksSourceRepository.GetRandomTracksSourceAsync(criteria);
            if (lastId == 0) {
                return await GrabAndSaveTracks(tracksSource);
            } else {
                List<Track> tracks = await tracksRepository.GetTracksAsync(criteria, 5, lastId);
                if (tracks.Count == 0) {
                    return await GrabAndSaveTracks(tracksSource);
                }
                return tracks;
            }
        }

        private async Task<List<Track>> GrabAndSaveTracks(TracksSource tracksSource) {
            List<Track> tracks = await grabber.GrabRandomTracksAsync(tracksSource);
            await tracksRepository.SaveTracksAsync(tracks);
            return tracks;
        }
    }
}
