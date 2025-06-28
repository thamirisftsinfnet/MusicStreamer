using MusicStreamer.Application.DTOs;
using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.AppServices
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(UserDto createUserDto);
        Task<string> AuthenticateAsync(LoginDto loginDto);
        Task<User> GetByIdAsync(int userId);
    }
}
