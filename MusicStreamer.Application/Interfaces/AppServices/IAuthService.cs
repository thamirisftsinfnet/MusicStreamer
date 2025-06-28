using MusicStreamer.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.AppServices
{
    public interface IAuthService
    {
        Task<string> HashPasswordAsync(string password);
        Task<bool> ValidatePasswordAsync(string password, string hashedPassword);
        Task<string> GenerateJwtTokenAsync(User user);
        Task<string> RenewToken(User user);
    }
}
