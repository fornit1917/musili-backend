using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Musili.WebApi.Interfaces;
using Musili.WebApi.Models;
using Musili.WebApi.Services.Grabbers.Yandex;

namespace Musili.Tests.Mocks
{
    public class YandexMusicClientMock : IYandexMusicClient
    {
        public Task<List<Track>> GetTracksByIdsAsync(List<int> ids)
        {
            List<Track> tracks = ids.Select(id => new Track() {
                Artist = "Artist",
                Title = $"Track{id}",
                Url = $"http://url-to-{id}",
            }).ToList();
            return Task.FromResult(tracks);
        }

        public Task<List<int>> GetTracksIdsByAlbumAsync(string albumId)
        {
            return Task.FromResult(GetTracksIds(albumId));
        }

        public Task<List<int>> GetTracksIdsByArtistAsync(string artistId)
        {
            return Task.FromResult(GetTracksIds(artistId));
        }

        public Task<List<int>> GetTracksIdsByUserPlaylistAsync(string userId, string playlistId)
        {
            return Task.FromResult(GetTracksIds(playlistId));
        }

        private List<int> GetTracksIds(string id) {
            int start = Int32.Parse(id);
            return new List<int> {
                start + 1,
                start + 2,
                start + 3,
                start + 4,
                start + 5,
            };
        }
    }
}