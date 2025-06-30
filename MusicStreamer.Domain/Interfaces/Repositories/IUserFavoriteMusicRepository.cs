using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.Repositories
{
    public interface IUserFavoriteMusicRepository : IRepository<UserFavoriteMusic>
    {
        Task<bool> IsFavoriteAsync(int userId, int musicId);
        Task AddFavoriteAsync(int userId, int musicId);
        Task RemoveFavoriteAsync(int userId, int musicId);
        Task<IEnumerable<UserFavoriteMusic>> GetIdUserAsync(int userId);
    }
}
