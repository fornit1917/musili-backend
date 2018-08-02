﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Musili.WebApi.Interfaces;
using Musili.WebApi.Models;
using Musili.WebApi.Models.Entities;

namespace Musili.WebApi.Services
{
    public class TracksProvider : ITracksProvider
    {
        public Task<List<Track>> GetTracksAsync(TracksCriteria criteria, int lastId = 0) {
            var result = new List<Track> {
                new Track(){ Artist = criteria.Genre.ToString(), Title = "Title 1", Url = "Url 1" },
                new Track(){ Artist = criteria.Tempo.ToString(), Title = "Title 2", Url = "Url 2" },
            };
            return Task.FromResult(result);
        }
    }
}