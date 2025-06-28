using MusicStreamer.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.AppServices
{
    public interface IMusicService
    {
        Task<IEnumerable<MusicSearchDto>> SearchMusicAsync(string searchTerm, int userId);
        Task<MusicDto> GetMusicByIdAsync(int musicId, int userId);
        Task ToggleFavoriteAsync(int musicId, int userId);
        Task<IEnumerable<MusicDto>> GetUserFavoritesAsync(int userId);
    }
}
