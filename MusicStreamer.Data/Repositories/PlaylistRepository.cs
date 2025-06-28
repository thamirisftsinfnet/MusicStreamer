using Microsoft.EntityFrameworkCore;
using MusicStreamer.Data.Context;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.Repositories
{
    public class PlaylistRepository : Repository<Playlist>, IPlaylistRepository
    {
        public PlaylistRepository(MusicStreamerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Playlist>> GetByUserAsync(int userId)
        {
            return await _dbSet
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Playlist> GetWithMusicsAsync(int playlistId)
        {
            return await _dbSet
                .Include(p => p.PlaylistMusics)
                    .ThenInclude(pm => pm.Music)
                        .ThenInclude(m => m.Album)
                            .ThenInclude(a => a.Band)
                .FirstOrDefaultAsync(p => p.Id == playlistId);
        }

        public async Task AddMusicToPlaylistAsync(int playlistId, int musicId)
        {
            var playlistMusic = new PlaylistMusic
            {
                PlaylistId = playlistId,
                MusicId = musicId,
                AddedAt = DateTime.UtcNow
            };

            await _context.PlaylistMusics.AddAsync(playlistMusic);
        }

        public async Task RemoveMusicFromPlaylistAsync(int playlistId, int musicId)
        {
            var playlistMusic = await _context.PlaylistMusics
                .FirstOrDefaultAsync(pm => pm.PlaylistId == playlistId && pm.MusicId == musicId);

            if (playlistMusic != null)
            {
                _context.PlaylistMusics.Remove(playlistMusic);
            }
        }
    }
}
