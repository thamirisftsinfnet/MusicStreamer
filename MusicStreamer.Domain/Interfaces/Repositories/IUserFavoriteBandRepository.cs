using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.Repositories
{
    public interface IUserFavoriteBandRepository : IRepository<UserFavoriteBand>
    {
        Task<bool> IsFavoriteAsync(int userId, int bandId);
        Task AddFavoriteAsync(int userId, int bandId);
        Task RemoveFavoriteAsync(int userId, int bandId);
        Task<IEnumerable<UserFavoriteBand>> GetIdUserAsync(int userId);
    }
}
