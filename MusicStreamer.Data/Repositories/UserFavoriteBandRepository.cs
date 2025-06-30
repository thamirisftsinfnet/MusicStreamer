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
    public class UserFavoriteBandRepository : Repository<UserFavoriteBand>, IUserFavoriteBandRepository
    {
        public UserFavoriteBandRepository(MusicStreamerContext context) : base(context)
        {
        }
        public async Task<IEnumerable<UserFavoriteBand>> GetIdUserAsync(int userId)
        {
            return await _dbSet.Where(ufm => ufm.UserId == userId).ToListAsync();
        }
        public async Task<bool> IsFavoriteAsync(int userId, int bandId)
        {
            return await _dbSet.AnyAsync(ufb => ufb.UserId == userId && ufb.BandId == bandId);
        }

        public async Task AddFavoriteAsync(int userId, int bandId)
        {
            if (!await IsFavoriteAsync(userId, bandId))
            {
                var favorite = new UserFavoriteBand
                {
                    UserId = userId,
                    BandId = bandId,
                    FavoritedAt = DateTime.UtcNow
                };

                await AddAsync(favorite);
            }
        }

        public async Task RemoveFavoriteAsync(int userId, int bandId)
        {
            var favorite = await _dbSet
                .FirstOrDefaultAsync(ufb => ufb.UserId == userId && ufb.BandId == bandId);

            if (favorite != null)
            {
                Remove(favorite);
            }
        }
    }
}
