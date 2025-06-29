using Microsoft.EntityFrameworkCore;
using MusicStreamer.Data.Context;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using MusicStreamer.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.Repositories
{
    public class CreditCardRepository : Repository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(MusicStreamerContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CreditCard>> GetByUserIdAsync(int userId)
        {
            return await _dbSet.Where(cc => cc.UserId == userId).ToListAsync();
        }
    }
}
