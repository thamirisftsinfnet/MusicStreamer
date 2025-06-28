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
    public class MusicRepository : Repository<Music>, IMusicRepository
    {
        public MusicRepository(MusicStreamerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Music>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Include(m => m.Album)
                    .ThenInclude(a => a.Band)
                .Where(m => m.Title.Contains(searchTerm) ||
                           m.Album.Title.Contains(searchTerm) ||
                           m.Album.Band.Name.Contains(searchTerm))
                .OrderBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Music>> GetByAlbumAsync(int albumId)
        {
            return await _dbSet
                .Where(m => m.AlbumId == albumId)
                .OrderBy(m => m.TrackNumber)
                .ThenBy(m => m.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Music>> GetUserFavoritesAsync(int userId)
        {
            return await _context.UserFavoriteMusics
                .Where(ufm => ufm.UserId == userId)
                .Include(ufm => ufm.Music)
                    .ThenInclude(m => m.Album)
                        .ThenInclude(a => a.Band)
                .Select(ufm => ufm.Music)
                .ToListAsync();
        }
    }
}
