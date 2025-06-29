using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Domain.Interfaces.Repositories
{
    public interface ICreditCardRepository : IRepository<CreditCard>
    {
        Task<IEnumerable<CreditCard>> GetByUserIdAsync(int userId);
    }
}
