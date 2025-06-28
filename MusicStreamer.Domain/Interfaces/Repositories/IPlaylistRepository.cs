using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.Repositories
{
    public interface IPlaylistRepository : IRepository<Playlist>
    {
        Task<IEnumerable<Playlist>> GetByUserAsync(int userId);
        Task<Playlist> GetWithMusicsAsync(int playlistId);
        Task AddMusicToPlaylistAsync(int playlistId, int musicId);
        Task RemoveMusicFromPlaylistAsync(int playlistId, int musicId);
    }
}
