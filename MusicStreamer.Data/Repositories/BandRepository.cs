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
    public class BandRepository : Repository<Band>, IBandRepository
    {
        public BandRepository(MusicStreamerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Band>> SearchAsync(string searchTerm)
        {
            return await _dbSet
                .Where(b => b.Name.Contains(searchTerm))
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Band>> GetUserFavoritesAsync(int userId)
        {
            return await _context.UserFavoriteBands
                .Where(ufb => ufb.UserId == userId)
                .Select(ufb => ufb.Band)
                .ToListAsync();
        }

        public async Task<Band> GetWithAlbumsAsync(int bandId)
        {
            return await _dbSet
                .Include(b => b.Albums)
                .FirstOrDefaultAsync(b => b.Id == bandId);
        }
    }
}
