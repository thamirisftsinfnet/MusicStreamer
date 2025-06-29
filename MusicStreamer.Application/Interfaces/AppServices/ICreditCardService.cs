using MusicStreamer.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Application.Interfaces.AppServices
{
    public interface ICreditCardService
    {
        Task<CreditCardDto> GetByIdAsync(int id);
        Task<IEnumerable<CreditCardDto>> GetByUserIdAsync(int userId);
        Task<CreditCardDto> AddCardAsync(CreditCardDto dto);
        Task DeleteCardAsync(CreditCardDto dto);
    }
}
