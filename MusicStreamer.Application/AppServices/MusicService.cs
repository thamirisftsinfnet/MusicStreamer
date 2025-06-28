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
    public class MusicService : IMusicService
    {
        private readonly IMusicRepository _musicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IBandRepository _bandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MusicService(
            IMusicRepository musicRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork, IAlbumRepository albumRepository, IBandRepository bandRepository)
        {
            _musicRepository = musicRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _albumRepository = albumRepository;
            _bandRepository = bandRepository;
        }

        public async Task<IEnumerable<MusicSearchDto>> SearchMusicAsync(string searchTerm, int userId)
        {
            IEnumerable<Music> musics;
            if(!string.IsNullOrEmpty(searchTerm))
            {
                musics = await _musicRepository.SearchAsync(searchTerm);
                if (!musics.Any())
                {
                    musics = await _musicRepository.GetAllAsync();
                }
            }
            else
            {
                musics = await _musicRepository.GetAllAsync();
            }

            var userFavorites = await _musicRepository.GetUserFavoritesAsync(userId);
            var favoriteIds = userFavorites.Select(m => m.Id).ToHashSet();
            musics.Select(m => m.Album = _albumRepository.GetByIdAsync(m.AlbumId).Result).ToList();
            musics.Select(m => m.Album.Band = _bandRepository.GetByIdAsync(m.Album.BandId).Result).ToList();

            return musics.Select(m => new MusicSearchDto
            {  
                Id = m.Id,
                Title = m.Title,
                BandName = m.Album.Band.Name,
                AlbumTitle = m.Album.Title,
                Duration = m.Duration,
                IsFavorite = favoriteIds.Contains(m.Id)
            });
        }

        public async Task<MusicDto> GetMusicByIdAsync(int musicId, int userId)
        {
            var music = await _musicRepository.GetByIdAsync(musicId);
            if (music == null)
                throw new InvalidOperationException("Música não encontrada");

            var userFavorites = await _musicRepository.GetUserFavoritesAsync(userId);
            var isFavorite = userFavorites.Any(m => m.Id == musicId);

            return new MusicDto
            {
                Id = music.Id,
                Title = music.Title,
                Duration = music.Duration,
                FileUrl = music.FileUrl,
                TrackNumber = music.TrackNumber,
                IsFavorite = isFavorite,
                Album = new AlbumDto
                {
                    Id = music.Album.Id,
                    Title = music.Album.Title,
                    ReleaseDate = music.Album.ReleaseDate,
                    CoverImageUrl = music.Album.CoverImageUrl,
                    Band = new BandDto
                    {
                        Id = music.Album.Band.Id,
                        Name = music.Album.Band.Name,
                        ImageUrl = music.Album.Band.ImageUrl
                    }
                }
            };
        }

        public async Task ToggleFavoriteAsync(int musicId, int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            var music = await _musicRepository.GetByIdAsync(musicId);

            if (user == null || music == null)
                throw new InvalidOperationException("Usuário ou música não encontrados");

            var existingFavorite = user.FavoriteMusics.FirstOrDefault(f => f.MusicId == musicId);

            if (existingFavorite != null)
            {
                user.FavoriteMusics.Remove(existingFavorite);
            }
            else
            {
                user.FavoriteMusics.Add(new UserFavoriteMusic
                {
                    UserId = userId,
                    MusicId = musicId,
                    FavoritedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<MusicDto>> GetUserFavoritesAsync(int userId)
        {
            var favoriteMusics = await _musicRepository.GetUserFavoritesAsync(userId);

            return favoriteMusics.Select(m => new MusicDto
            {
                Id = m.Id,
                Title = m.Title,
                Duration = m.Duration,
                FileUrl = m.FileUrl,
                TrackNumber = m.TrackNumber,
                IsFavorite = true,
                Album = new AlbumDto
                {
                    Id = m.Album.Id,
                    Title = m.Album.Title,
                    ReleaseDate = m.Album.ReleaseDate,
                    CoverImageUrl = m.Album.CoverImageUrl,
                    Band = new BandDto
                    {
                        Id = m.Album.Band.Id,
                        Name = m.Album.Band.Name,
                        ImageUrl = m.Album.Band.ImageUrl
                    }
                }
            });
        }
    }
}
