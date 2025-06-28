using MusicStreamer.Application.DTOs;
using MusicStreamer.Application.Interfaces.AppServices;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Domain.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.AppServices
{
    public class BandService : IBandService
    {
        private readonly IBandRepository _bandRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserFavoriteBandRepository _userFavoriteBandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public BandService(
            IBandRepository bandRepository,
            IUserRepository userRepository,
            IUserFavoriteBandRepository userFavoriteBandRepository,
            IUnitOfWork unitOfWork)
        {
            _bandRepository = bandRepository;
            _userRepository = userRepository;
            _userFavoriteBandRepository = userFavoriteBandRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BandSearchDto>> SearchBandsAsync(string searchTerm, int userId)
        {
            IEnumerable<Band> _bands;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                _bands = await _bandRepository.SearchAsync(searchTerm);
                if (!_bands.Any())
                {
                    _bands = await _bandRepository.GetAllAsync();
                }
            }
            else
            {
                _bands = await _bandRepository.GetAllAsync();
            }

                var userFavoriteBands = await _bandRepository.GetUserFavoritesAsync(userId);
            var favoriteBandIds = userFavoriteBands.Select(b => b.Id).ToHashSet();

            return _bands.Select(b => new BandSearchDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                IsFavorite = favoriteBandIds.Contains(b.Id),
                AlbumsCount = b.Albums?.Count ?? 0
            });
        }

        public async Task<BandDto> GetBandByIdAsync(int bandId, int userId)
        {
            var band = await _bandRepository.GetWithAlbumsAsync(bandId);
            if (band == null)
                throw new InvalidOperationException("Banda não encontrada");

            var isFavorite = await _userFavoriteBandRepository.IsFavoriteAsync(userId, bandId);

            return new BandDto
            {
                Id = band.Id,
                Name = band.Name,
                Description = band.Description,
                ImageUrl = band.ImageUrl,
                CreatedAt = band.CreatedAt,
                IsFavorite = isFavorite,
                Albums = band.Albums?.Select(a => new AlbumDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ReleaseDate = a.ReleaseDate,
                    CoverImageUrl = a.CoverImageUrl
                }).ToList() ?? new List<AlbumDto>()
            };
        }

        public async Task<IEnumerable<BandDto>> GetUserFavoriteBandsAsync(int userId)
        {
            var favoriteBands = await _bandRepository.GetUserFavoritesAsync(userId);

            return favoriteBands.Select(b => new BandDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                CreatedAt = b.CreatedAt,
                IsFavorite = true,
                Albums = b.Albums?.Select(a => new AlbumDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ReleaseDate = a.ReleaseDate,
                    CoverImageUrl = a.CoverImageUrl
                }).ToList() ?? new List<AlbumDto>()
            });
        }

        public async Task AddToFavoritesAsync(int userId, int bandId)
        {
            var band = await _bandRepository.GetByIdAsync(bandId);
            if (band == null)
                throw new InvalidOperationException("Banda não encontrada");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("Usuário não encontrado");

            await _userFavoriteBandRepository.AddFavoriteAsync(userId, bandId);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveFromFavoritesAsync(int userId, int bandId)
        {
            await _userFavoriteBandRepository.RemoveFavoriteAsync(userId, bandId);
            await _unitOfWork.CommitAsync();
        }

        public async Task<BandDto> CreateBandAsync(BandDto createBandDto)
        {
            var band = new Band
            {
                Name = createBandDto.Name,
                Description = createBandDto.Description,
                ImageUrl = createBandDto.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _bandRepository.AddAsync(band);
            await _unitOfWork.CommitAsync();

            return new BandDto
            {
                Id = band.Id,
                Name = band.Name,
                Description = band.Description,
                ImageUrl = band.ImageUrl,
                CreatedAt = band.CreatedAt,
                IsFavorite = false,
                Albums = new List<AlbumDto>()
            };
        }

        public async Task<BandDto> UpdateBandAsync(int bandId, BandDto updateBandDto)
        {
            var band = await _bandRepository.GetByIdAsync(bandId);
            if (band == null)
                throw new InvalidOperationException("Banda não encontrada");

            band.Name = updateBandDto.Name ?? band.Name;
            band.Description = updateBandDto.Description ?? band.Description;
            band.ImageUrl = updateBandDto.ImageUrl ?? band.ImageUrl;

            _bandRepository.Update(band);
            await _unitOfWork.CommitAsync();

            return new BandDto
            {
                Id = band.Id,
                Name = band.Name,
                Description = band.Description,
                ImageUrl = band.ImageUrl,
                CreatedAt = band.CreatedAt,
                IsFavorite = false,
                Albums = band.Albums?.Select(a => new AlbumDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ReleaseDate = a.ReleaseDate,
                    CoverImageUrl = a.CoverImageUrl
                }).ToList() ?? new List<AlbumDto>()
            };
        }

        public async Task DeleteBandAsync(int bandId)
        {
            var band = await _bandRepository.GetByIdAsync(bandId);
            if (band == null)
                throw new InvalidOperationException("Banda não encontrada");

            _bandRepository.Remove(band);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<BandDto>> GetAllBandsAsync()
        {
            var bands = await _bandRepository.GetAllAsync();

            return bands.Select(b => new BandDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageUrl,
                CreatedAt = b.CreatedAt,
                IsFavorite = false,
                Albums = b.Albums?.Select(a => new AlbumDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ReleaseDate = a.ReleaseDate,
                    CoverImageUrl = a.CoverImageUrl
                }).ToList() ?? new List<AlbumDto>()
            });
        }
    }
}