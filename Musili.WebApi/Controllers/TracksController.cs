﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Musili.WebApi.Interfaces;
using Musili.WebApi.Models;
using Musili.WebApi.Models.Entities;

namespace Musili.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class TracksController : Controller
    {
        private ITracksProvider tracksProvider;

        public TracksController(ITracksProvider tracksProvider) {
            this.tracksProvider = tracksProvider;
        }

        [HttpGet("")]
        public async Task<List<Track>> GetTracks(string tempos, string genres, int lastId = 0) {
            TracksCriteriaSet criteriaSet = new TracksCriteriaSet(tempos, genres);
            return await tracksProvider.GetTracksAsync(criteriaSet.GetRandomCriteria(), lastId);
        }

        [HttpGet("hello/{name}")]
        public string Hello(string name) {
            return $"Hello, {name}!";
        }

        [HttpGet("test")]
        public Track Test() {
            return new Track();
        }
    }
}
