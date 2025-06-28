using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.Repositories
{
    public interface IBandRepository : IRepository<Band>
    {
        Task<IEnumerable<Band>> SearchAsync(string searchTerm);
        Task<IEnumerable<Band>> GetUserFavoritesAsync(int userId);
        Task<Band> GetWithAlbumsAsync(int bandId);

    }
}
