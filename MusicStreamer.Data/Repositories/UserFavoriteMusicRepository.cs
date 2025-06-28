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
    public class UserFavoriteMusicRepository : Repository<UserFavoriteMusic>, IUserFavoriteMusicRepository
    {
        public UserFavoriteMusicRepository(MusicStreamerContext context) : base(context)
        {
        }

        public async Task<bool> IsFavoriteAsync(int userId, int musicId)
        {
            return await _dbSet.AnyAsync(ufm => ufm.UserId == userId && ufm.MusicId == musicId);
        }

        public async Task AddFavoriteAsync(int userId, int musicId)
        {
            if (!await IsFavoriteAsync(userId, musicId))
            {
                var favorite = new UserFavoriteMusic
                {
                    UserId = userId,
                    MusicId = musicId,
                    FavoritedAt = DateTime.UtcNow
                };

                await AddAsync(favorite);
            }
        }

        public async Task RemoveFavoriteAsync(int userId, int musicId)
        {
            var favorite = await _dbSet
                .FirstOrDefaultAsync(ufm => ufm.UserId == userId && ufm.MusicId == musicId);

            if (favorite != null)
            {
                Remove(favorite);
            }
        }
    }
}
