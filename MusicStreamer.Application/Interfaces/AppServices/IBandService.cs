using MusicStreamer.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.AppServices
{
    public interface IBandService
    {
        Task<IEnumerable<BandSearchDto>> SearchBandsAsync(string searchTerm, int userId);
        Task<BandDto> GetBandByIdAsync(int bandId, int userId);
        Task<IEnumerable<BandDto>> GetUserFavoriteBandsAsync(int userId);
        Task AddToFavoritesAsync(int userId, int bandId);
        Task RemoveFromFavoritesAsync(int userId, int bandId);
        Task<BandDto> CreateBandAsync(BandDto createBandDto);
        Task<BandDto> UpdateBandAsync(int bandId, BandDto updateBandDto);
        Task ToggleFavoriteAsync(int bandId, int userId);
    }
}
