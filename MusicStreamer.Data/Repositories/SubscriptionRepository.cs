using Microsoft.EntityFrameworkCore;
using MusicStreamer.Data.Context;
using MusicStreamer.Domain.Entities;
using MusicStreamer.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStreamer.Data.Repositories
{
    public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(MusicStreamerContext context) : base(context)
        {
        }

        public async Task<Subscription> GetByUserIdAsync(int userId)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive && s.EndDate > DateTime.UtcNow)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetExpiredSubscriptionsAsync()
        {
            return await _dbSet
                .Where(s => s.IsActive && s.EndDate <= DateTime.UtcNow)
                .Include(s => s.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Subscription>> GetExpiringSubscriptionsAsync(int daysFromNow)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysFromNow);
            return await _dbSet
                .Where(s => s.IsActive && s.EndDate <= targetDate && s.EndDate > DateTime.UtcNow)
                .Include(s => s.User)
                .ToListAsync();
        }
    }
}
